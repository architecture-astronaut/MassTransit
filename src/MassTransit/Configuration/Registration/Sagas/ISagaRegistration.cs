namespace MassTransit.Registration.Sagas
{
    using System;
    using Definition;
    using MassTransit.Saga;


    public interface ISagaRegistration
    {
        Type SagaType { get; }

        void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga;

        void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider repositoryFactory);

        ISagaDefinition GetDefinition(IConfigurationServiceProvider provider);
    }
}
