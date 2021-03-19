namespace MassTransit.RedisIntegration
{
    public interface ConsumeResultContext
    {
        string Stream { get; }
        Offset Offset { get; }
        Timestamp Timestamp { get; }
        Headers Headers { get; }
    }
}
