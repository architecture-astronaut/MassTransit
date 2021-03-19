namespace MassTransit.RedisIntegration.Transport
{
    using GreenPipes.Agents;
    using Transports.Metrics;


    public interface IRedisMessageReceiver :
        IAgent,
        DeliveryMetrics
    {
    }


    public interface IRedisMessageReceiver<TKey, TValue> :
        IRedisMessageReceiver
        where TValue : class
    {
    }
}
