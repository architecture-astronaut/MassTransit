namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface BurgerCompleted :
        OrderLineCompleted
    {
        Burger Burger { get; }
    }
}