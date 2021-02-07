namespace MassTransit.Conductor.Observers
{
    using EndpointConfigurators;
    using Inventory;


    public class ServiceRegistryConfigurationObserver :
        IEndpointConfigurationObserver
    {
        readonly IBusFactoryConfigurator _configurator;
        readonly ServiceRegistry _registry;

        public ServiceRegistryConfigurationObserver(IBusFactoryConfigurator configurator, ServiceRegistry registry)
        {
            _configurator = configurator;
            _registry = registry;
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            var observer = new ServiceRegistryEndpointConfigurationObserver(_configurator, _registry, configurator);

            _registry.AddConfigurationHandle(configurator.ConnectConsumerConfigurationObserver(observer));
            _registry.AddConfigurationHandle(configurator.ConnectHandlerConfigurationObserver(observer));
            _registry.AddConfigurationHandle(configurator.ConnectSagaConfigurationObserver(observer));
            _registry.AddConfigurationHandle(configurator.ConnectActivityConfigurationObserver(observer));
        }
    }
}
