namespace MassTransit.RedisIntegration.Contexts
{
    using System.Threading.Tasks;


    public interface IConsumerLockContext<TKey, TValue>
    {
        Task Complete(ConsumeResult<TKey, TValue> result);
    }
}
