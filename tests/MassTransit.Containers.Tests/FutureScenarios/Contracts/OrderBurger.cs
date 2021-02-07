namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderBurger :
        OrderLine
    {
        Burger Burger { get; }
    }
}