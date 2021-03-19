namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Serializers;
    using Transport;


    public class RedisProducerContext<TKey, TValue> :
        BasePipeContext,
        ProducerContext<TKey, TValue>
        where TValue : class
    {
        readonly IProducer<TKey, TValue> _producer;

        public RedisProducerContext(ProducerBuilder<TKey, TValue> producerBuilder, IHeadersSerializer headersSerializer, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _producer = producerBuilder.Build();
            HeadersSerializer = headersSerializer;
        }

        public IHeadersSerializer HeadersSerializer { get; }

        public Task Produce(StreamPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
        {
            return _producer.ProduceAsync(partition, message, cancellationToken);
        }

        public void Dispose()
        {
            var timeout = TimeSpan.FromSeconds(30);
            _producer.Flush(timeout);
            _producer.Dispose();
        }
    }
}
