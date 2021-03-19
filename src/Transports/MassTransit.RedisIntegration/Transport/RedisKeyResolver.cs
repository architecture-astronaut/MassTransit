namespace MassTransit.RedisIntegration.Transport
{
    public delegate TKey RedisKeyResolver<out TKey, in TValue>(RedisSendContext<TValue> context)
        where TValue : class;
}
