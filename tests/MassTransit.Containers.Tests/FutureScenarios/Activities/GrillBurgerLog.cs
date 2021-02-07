namespace MassTransit.Containers.Tests.FutureScenarios.Activities
{
    using Contracts;


    public interface GrillBurgerLog
    {
        BurgerPatty Patty { get; }
    }
}
