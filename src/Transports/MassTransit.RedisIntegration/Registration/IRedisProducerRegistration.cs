namespace MassTransit.RedisIntegration.Registration
{
    using MassTransit.Registration;


    public interface IRedisProducerRegistration
    {
        void Register(IRedisFactoryConfigurator configurator, IRiderRegistrationContext context);
    }
}
