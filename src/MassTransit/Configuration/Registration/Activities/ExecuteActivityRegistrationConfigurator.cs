namespace MassTransit.Registration.Activities
{
    using System;
    using Definition;
    using MassTransit.Courier;


    public class ExecuteActivityRegistrationConfigurator<TActivity, TArguments> :
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IRegistrationConfigurator _configurator;

        public ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Endpoint(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configure)
        {
            var configurator = new ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(configurator.Settings);
        }
    }
}
