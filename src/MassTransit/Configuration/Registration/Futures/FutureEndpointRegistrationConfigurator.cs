namespace MassTransit.Registration.Futures
{
    using Automatonymous;
    using Endpoints;
    using MassTransit.Futures;


    public class FutureEndpointRegistrationConfigurator<TFuture> :
        EndpointRegistrationConfigurator<TFuture>,
        IFutureEndpointRegistrationConfigurator<TFuture>
        where TFuture : MassTransitStateMachine<FutureState>
    {
    }
}
