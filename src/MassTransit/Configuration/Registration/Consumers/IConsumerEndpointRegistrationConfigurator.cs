namespace MassTransit.Registration.Consumers
{
    using Endpoints;


    public interface IConsumerEndpointRegistrationConfigurator<TConsumer> :
        IEndpointRegistrationConfigurator
        where TConsumer : class, IConsumer
    {
    }
}
