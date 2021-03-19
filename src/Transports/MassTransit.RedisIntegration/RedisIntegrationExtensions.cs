namespace MassTransit
{
    using System;
    using RedisIntegration;
    using RedisIntegration.Registration;
    using Registration;


    public static class RedisIntegrationExtensions
    {
        public static void UsingRedis(this IRiderRegistrationConfigurator configurator, Action<IRiderRegistrationContext, IRedisFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new RedisRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);
        }

        public static void UsingRedis(this IRiderRegistrationConfigurator configurator, ClientConfig clientConfig,
            Action<IRiderRegistrationContext, IRedisFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (clientConfig == null)
                throw new ArgumentNullException(nameof(clientConfig));

            var factory = new RedisRegistrationRiderFactory(clientConfig, configure);
            configurator.SetRiderFactory(factory);
        }
    }
}
