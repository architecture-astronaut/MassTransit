namespace MassTransit.RedisIntegration.Transport
{
    public interface IRedisProducerFactory
    {
    }


    public interface IRedisProducerFactory<TKey, TValue> :
        IRedisProducerFactory
        where TValue : class
    {
        IStreamProducer<TKey, TValue> CreateProducer();
    }
}
