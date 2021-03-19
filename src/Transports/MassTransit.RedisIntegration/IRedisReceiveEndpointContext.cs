namespace MassTransit.RedisIntegration
{
    using Context;
    using Contexts;


    public interface IRedisReceiveEndpointContext<TKey, TValue> :
        ReceiveEndpointContext
        where TValue : class
    {
        IConsumerContextSupervisor<TKey, TValue> ConsumerContextSupervisor { get; }
    }
}
