namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Initializers;
    using Logging;
    using Transports;


    public class StreamProducer<TKey, TValue> :
        IStreamProducer<TKey, TValue>
        where TValue : class
    {
        readonly RedisSendTransportContext _context;
        readonly IProducerContextSupervisor<TKey, TValue> _supervisor;

        public StreamProducer(RedisSendTransportContext context, IProducerContextSupervisor<TKey, TValue> supervisor)
        {
            _context = context;
            _supervisor = supervisor;
        }

        public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            return Produce(key, value, Pipe.Empty<RedisSendContext<TKey, TValue>>(), cancellationToken);
        }

        public Task Produce(TKey key, TValue value, IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken)
        {
            var sendPipe = new SendPipe(_context, key, value, pipe, cancellationToken);

            return _supervisor.Send(sendPipe, cancellationToken);
        }

        public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
        {
            return Produce(key, values, Pipe.Empty<RedisSendContext<TKey, TValue>>(), cancellationToken);
        }

        public Task Produce(TKey key, object values, IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            Task<InitializeContext<TValue>> messageTask = MessageInitializerCache<TValue>.Initialize(values, cancellationToken);
            if (messageTask.IsCompletedSuccessfully())
                return Produce(key, messageTask.GetAwaiter().GetResult().Message, pipe, cancellationToken);

            async Task ProduceAsync()
            {
                InitializeContext<TValue> context = await messageTask.ConfigureAwait(false);

                await Produce(key, context.Message, pipe, cancellationToken).ConfigureAwait(false);
            }

            return ProduceAsync();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }


        class SendPipe :
            IPipe<ProducerContext<TKey, TValue>>
        {
            readonly CancellationToken _cancellationToken;
            readonly RedisSendTransportContext _context;
            readonly TKey _key;
            readonly IPipe<RedisSendContext<TKey, TValue>> _pipe;
            readonly TValue _value;

            public SendPipe(RedisSendTransportContext context, TKey key, TValue value,
                IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken)
            {
                _context = context;
                _key = key;
                _value = value;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ProducerContext<TKey, TValue> context)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                var sendContext = new RedisMessageSendContext<TKey, TValue>(_key, _value, _cancellationToken) {DestinationAddress = _context.StreamAddress};

                await _context.SendPipe.Send(sendContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(sendContext).ConfigureAwait(false);

                sendContext.SourceAddress ??= _context.HostAddress;
                sendContext.ConversationId ??= NewId.NextGuid();

                StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(sendContext,
                    (nameof(sendContext.Partition), sendContext.Partition.ToString()));
                try
                {
                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(sendContext).ConfigureAwait(false);

                    var message = new Message<TKey, TValue>
                    {
                        Key = sendContext.Key,
                        Value = sendContext.Message
                    };

                    if (sendContext.SentTime.HasValue)
                        message.Timestamp = new Timestamp(sendContext.SentTime.Value);

                    message.Headers = context.HeadersSerializer.Serialize(sendContext);

                    var Stream = new StreamPartition(_context.StreamAddress.Stream, sendContext.Partition);

                    await context.Produce(Stream, message, context.CancellationToken).ConfigureAwait(false);

                    sendContext.LogSent();
                    activity.AddSendContextHeadersPostSend(sendContext);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PostSend(sendContext).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    sendContext.LogFaulted(exception);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.SendFault(sendContext, exception).ConfigureAwait(false);

                    throw;
                }
                finally
                {
                    activity?.Stop();
                }
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
