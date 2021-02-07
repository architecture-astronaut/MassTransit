namespace MassTransit.Registration
{
    using Activities;
    using Consumers;
    using Futures;
    using Sagas;


    public interface IRegistrationFilter
    {
        bool Matches(IConsumerRegistration registration);
        bool Matches(ISagaRegistration registration);
        bool Matches(IExecuteActivityRegistration registration);
        bool Matches(IActivityRegistration registration);
        bool Matches(IFutureRegistration registration);
    }
}
