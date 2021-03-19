namespace MassTransit.RedisIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class KeyedStreamProducer<TKey, TValue> :
        IStreamProducer<TValue>
        where TValue : class
    {
        readonly RedisKeyResolver<TKey, TValue> _keyResolver;
        readonly IStreamProducer<TKey, TValue> _StreamProducer;

        public KeyedStreamProducer(IStreamProducer<TKey, TValue> StreamProducer, RedisKeyResolver<TKey, TValue> keyResolver)
        {
            _StreamProducer = StreamProducer;
            _keyResolver = keyResolver;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _StreamProducer.ConnectSendObserver(observer);
        }

        public Task Produce(TValue message, CancellationToken cancellationToken = default)
        {
            return Produce(message, Pipe.Empty<RedisSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce(TValue message, IPipe<RedisSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
        {
            return _StreamProducer.Produce(default, message, new SetKeyPipe(_keyResolver, pipe), cancellationToken);
        }

        public Task Produce(object values, CancellationToken cancellationToken = default)
        {
            return Produce(values, Pipe.Empty<RedisSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce(object values, IPipe<RedisSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
        {
            return _StreamProducer.Produce(default, values, new SetKeyPipe(_keyResolver, pipe), cancellationToken);
        }


        class SetKeyPipe :
            IPipe<RedisSendContext<TKey, TValue>>
        {
            readonly RedisKeyResolver<TKey, TValue> _keyResolver;
            readonly IPipe<RedisSendContext<TKey, TValue>> _pipe;

            public SetKeyPipe(RedisKeyResolver<TKey, TValue> keyResolver, IPipe<RedisSendContext<TKey, TValue>> pipe = null)
            {
                _keyResolver = keyResolver;
                _pipe = pipe;
            }

            public Task Send(RedisSendContext<TKey, TValue> context)
            {
                context.Key = _keyResolver(context);
                return _pipe.IsNotEmpty() ? _pipe.Send(context) : TaskUtil.Completed;
            }

            public void Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }
        }
    }
}
