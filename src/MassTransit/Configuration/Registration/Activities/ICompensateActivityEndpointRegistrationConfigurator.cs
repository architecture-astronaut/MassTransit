namespace MassTransit.Registration.Activities
{
    using Endpoints;
    using MassTransit.Courier;


    public interface ICompensateActivityEndpointRegistrationConfigurator<TActivity, TLog> :
        IEndpointRegistrationConfigurator
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
    }
}
