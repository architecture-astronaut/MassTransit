namespace MassTransit.RedisIntegration.Registration
{
    using System;
    using Configurators;
    using MassTransit.Registration;
    using Transport;


    public class RedisRegistrationRiderFactory :
        IRegistrationRiderFactory<IRedisRider>
    {
        readonly ClientConfig _clientConfig;
        readonly Action<IRiderRegistrationContext, IRedisFactoryConfigurator> _configure;

        public RedisRegistrationRiderFactory(Action<IRiderRegistrationContext, IRedisFactoryConfigurator> configure)
            : this(null, configure)
        {
        }

        public RedisRegistrationRiderFactory(ClientConfig clientConfig,
            Action<IRiderRegistrationContext, IRedisFactoryConfigurator> configure)
        {
            _clientConfig = clientConfig;
            _configure = configure;
        }

        public IBusInstanceSpecification CreateRider(IRiderRegistrationContext context)
        {
            var configurator = new RedisFactoryConfigurator(_clientConfig ?? context.GetService<ClientConfig>() ?? new ClientConfig());

            _configure?.Invoke(context, configurator);

            foreach (var registration in context.GetRegistrations<IRedisProducerRegistration>())
                registration.Register(configurator, context);

            return configurator.Build(context);
        }
    }
}
