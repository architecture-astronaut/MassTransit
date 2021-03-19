namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Serializers;


    public interface ProducerContext<TKey, TValue> :
        PipeContext,
        IDisposable
        where TValue : class
    {
        IHeadersSerializer HeadersSerializer { get; }

        Task Produce(StreamPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken);
    }
}
