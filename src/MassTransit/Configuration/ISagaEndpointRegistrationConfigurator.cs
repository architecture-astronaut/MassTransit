namespace MassTransit
{
    using Registration.Endpoints;
    using Saga;


    public interface ISagaEndpointRegistrationConfigurator<TSaga> :
        IEndpointRegistrationConfigurator
        where TSaga : class, ISaga
    {
    }
}
