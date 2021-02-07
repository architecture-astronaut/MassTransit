namespace MassTransit.Registration.Futures
{
    using System;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Context;
    using MassTransit.Futures;
    using Metadata;
    using Saga;
    using Sagas;


    public class FutureRegistration<TFuture> :
        IFutureRegistration
        where TFuture : MassTransitStateMachine<FutureState>
    {
        IFutureDefinition<TFuture> _definition;

        public Type FutureType => typeof(TFuture);

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var stateMachine = configurationServiceProvider.GetRequiredService<TFuture>();
            var repository = configurationServiceProvider.GetRequiredService<ISagaRepository<FutureState>>();

            var decoratorRegistration = configurationServiceProvider.GetService<ISagaRepositoryDecoratorRegistration<FutureState>>();
            if (decoratorRegistration != null)
                repository = decoratorRegistration.DecorateSagaRepository(repository);

            var sagaConfigurator = new StateMachineSagaConfigurator<FutureState>(stateMachine, repository, configurator);

            GetFutureDefinition(configurationServiceProvider)
                .Configure(configurator, sagaConfigurator);

            LogContext.Debug?.Log("Configured endpoint {Endpoint}, Saga: {SagaType}, State Machine: {StateMachineType}",
                configurator.InputAddress.GetLastPart(),
                TypeMetadataCache<FutureState>.ShortName, TypeMetadataCache.GetShortName(stateMachine.GetType()));

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        public IFutureDefinition GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetFutureDefinition(provider);
        }

        IFutureDefinition<TFuture> GetFutureDefinition(IConfigurationServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<IFutureDefinition<TFuture>>() ?? new DefaultFutureDefinition<TFuture>();

            var endpointDefinition = provider.GetService<IEndpointDefinition<TFuture>>();
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}
