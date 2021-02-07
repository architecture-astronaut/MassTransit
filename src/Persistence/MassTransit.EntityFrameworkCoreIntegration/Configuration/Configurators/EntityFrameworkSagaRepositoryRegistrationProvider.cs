namespace MassTransit.EntityFrameworkCoreIntegration.Configurators
{
    using System;
    using MassTransit.Saga;
    using Registration;
    using Registration.Sagas;


    public class EntityFrameworkSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IEntityFrameworkSagaRepositoryConfigurator> _configure;

        public EntityFrameworkSagaRepositoryRegistrationProvider(Action<IEntityFrameworkSagaRepositoryConfigurator> configure)
        {
            _configure = configure;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.EntityFrameworkRepository(r => _configure?.Invoke(r));
        }
    }
}
