namespace MassTransit
{
    using Automatonymous;
    using Futures;
    using Registration.Endpoints;


    public interface IFutureEndpointRegistrationConfigurator<TFuture> :
        IEndpointRegistrationConfigurator
        where TFuture : MassTransitStateMachine<FutureState>
    {
    }
}
