namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderCalculate :
        OrderLine
    {
        int Number { get; }
    }
}
