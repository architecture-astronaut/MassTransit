namespace MassTransit
{
    using System;
    using Courier;
    using Registration;
    using Registration.Activities;


    public interface IExecuteActivityRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        void Endpoint(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configure);
    }
}
