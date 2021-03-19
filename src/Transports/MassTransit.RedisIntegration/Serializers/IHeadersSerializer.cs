namespace MassTransit.RedisIntegration.Serializers
{
    public interface IHeadersSerializer
    {
        Headers Serialize(SendContext context);
    }
}
