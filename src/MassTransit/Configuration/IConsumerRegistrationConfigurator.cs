namespace MassTransit
{
    using System;
    using Registration;
    using Registration.Consumers;


    public interface IConsumerRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
        void Endpoint(Action<IConsumerEndpointRegistrationConfigurator<TConsumer>> configure);
    }
}
