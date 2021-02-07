namespace MassTransit.Registration.Consumers
{
    using Endpoints;


    public class ConsumerEndpointRegistrationConfigurator<TConsumer> :
        EndpointRegistrationConfigurator<TConsumer>,
        IConsumerEndpointRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
    }
}
