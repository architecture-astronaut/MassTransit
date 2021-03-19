namespace MassTransit.RedisIntegration.Contexts
{
    using System.Threading;
    using Context;


    public class RedisMessageSendContext<TKey, T> :
        MessageSendContext<T>,
        RedisSendContext<TKey, T>
        where T : class
    {
        public RedisMessageSendContext(TKey key, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            Key = key;
            Partition = Partition.Any;
        }
        
        public TKey Key { get; set; }
    }
}
