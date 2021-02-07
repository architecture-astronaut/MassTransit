namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderCombo :
        OrderLine
    {
        int Number { get; }
    }
}
