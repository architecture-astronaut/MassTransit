namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderFry :
        OrderLine
    {
        Size Size { get; }
    }
}