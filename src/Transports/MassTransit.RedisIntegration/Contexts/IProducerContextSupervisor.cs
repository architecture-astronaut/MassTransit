namespace MassTransit.RedisIntegration.Contexts
{
    using Transport;
    using Transports;


    public interface IProducerContextSupervisor<TKey, TValue> :
        ITransportSupervisor<ProducerContext<TKey, TValue>>,
        IRedisProducerFactory<TKey, TValue>
        where TValue : class
    {
    }
}
