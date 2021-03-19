namespace MassTransit.RedisIntegration.Contexts
{
    using System.Threading;
    using GreenPipes;
    using StackExchange.Redis;


    public class RedisClientContext :
        BasePipeContext,
        ClientContext
    {
        public RedisClientContext(ConfigurationOptions config, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Config = config;
        }

        public ConfigurationOptions Config { get; }
    }
}
