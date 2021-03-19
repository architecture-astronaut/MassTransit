namespace MassTransit.RedisIntegration
{
    public interface RedisSendContext :
        SendContext
    {
    }


    public interface RedisSendContext<out T> :
        SendContext<T>,
        RedisSendContext
        where T : class
    {
    }


    public interface RedisSendContext<TKey, out T> :
        RedisSendContext<T>
        where T : class
    {
        TKey Key { get; set; }
    }
}
