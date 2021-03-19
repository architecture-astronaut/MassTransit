namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;


    public class ConsumeContextStreamProducerProvider :
        IStreamProducerProvider
    {
        readonly ConsumeContext _consumeContext;
        readonly IStreamProducerProvider _provider;

        public ConsumeContextStreamProducerProvider(IStreamProducerProvider provider, ConsumeContext consumeContext)
        {
            _provider = provider;
            _consumeContext = consumeContext;
        }

        public IStreamProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class
        {
            IStreamProducer<TKey, TValue> producer = _provider.GetProducer<TKey, TValue>(address);
            return new StreamProducer<TKey, TValue>(producer, _consumeContext);
        }


        class StreamProducer<TKey, TValue> :
            IStreamProducer<TKey, TValue>
            where TValue : class
        {
            readonly ConsumeContext _consumeContext;
            readonly IStreamProducer<TKey, TValue> _producer;

            public StreamProducer(IStreamProducer<TKey, TValue> producer, ConsumeContext consumeContext)
            {
                _producer = producer;
                _consumeContext = consumeContext;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _producer.ConnectSendObserver(observer);
            }

            public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
            {
                return Produce(key, value, Pipe.Empty<RedisSendContext<TKey, TValue>>(), cancellationToken);
            }

            public Task Produce(TKey key, TValue value, IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter(pipe, _consumeContext);

                return _producer.Produce(key, value, sendPipeAdapter, cancellationToken);
            }

            public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
            {
                return Produce(key, values, Pipe.Empty<RedisSendContext<TKey, TValue>>(), cancellationToken);
            }

            public Task Produce(TKey key, object values, IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter(pipe, _consumeContext);

                return _producer.Produce(key, values, sendPipeAdapter, cancellationToken);
            }


            class ConsumeSendPipeAdapter :
                IPipe<RedisSendContext<TKey, TValue>>,
                ISendPipe
            {
                readonly ConsumeContext _consumeContext;
                readonly IPipe<RedisSendContext<TKey, TValue>> _pipe;

                public ConsumeSendPipeAdapter(IPipe<RedisSendContext<TKey, TValue>> pipe, ConsumeContext consumeContext)
                {
                    _pipe = pipe;
                    _consumeContext = consumeContext;
                }

                public async Task Send(RedisSendContext<TKey, TValue> context)
                {
                    if (_consumeContext != null)
                        context.TransferConsumeContextHeaders(_consumeContext);

                    if (_pipe.IsNotEmpty())
                        await _pipe.Send(context).ConfigureAwait(false);
                }

                public void Probe(ProbeContext context)
                {
                    _pipe.Probe(context);
                }

                public async Task Send<T>(SendContext<T> context)
                    where T : class
                {
                }
            }
        }
    }
}
