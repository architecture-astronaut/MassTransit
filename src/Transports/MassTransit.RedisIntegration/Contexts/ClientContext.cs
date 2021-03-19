namespace MassTransit.RedisIntegration.Contexts
{
    using GreenPipes;
    using StackExchange.Redis;


    public interface ClientContext :
        PipeContext
    {
        ConfigurationOptions Config { get; }
    }
}
