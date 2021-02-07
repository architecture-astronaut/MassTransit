namespace MassTransit.Registration.Activities
{
    using Endpoints;
    using MassTransit.Courier;


    public class ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments> :
        EndpointRegistrationConfigurator<IExecuteActivity<TArguments>>,
        IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public ExecuteActivityEndpointRegistrationConfigurator()
        {
            ConfigureConsumeTopology = false;
        }
    }
}
