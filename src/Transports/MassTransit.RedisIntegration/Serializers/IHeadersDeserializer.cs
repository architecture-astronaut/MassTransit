namespace MassTransit.RedisIntegration.Serializers
{
    using Context;


    public interface IHeadersDeserializer
    {
        IHeaderProvider Deserialize(Headers headers);
    }
}
