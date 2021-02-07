namespace MassTransit.Registration.Activities
{
    using Endpoints;
    using MassTransit.Courier;


    public interface IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments> :
        IEndpointRegistrationConfigurator
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
    }
}
