namespace MassTransit.Registration.Sagas
{
    using Endpoints;
    using Saga;


    public class SagaEndpointRegistrationConfigurator<TSaga> :
        EndpointRegistrationConfigurator<TSaga>,
        ISagaEndpointRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
    }
}
