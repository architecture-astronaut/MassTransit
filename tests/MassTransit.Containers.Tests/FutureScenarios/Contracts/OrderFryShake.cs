namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderFryShake :
        OrderLine
    {
        string Flavor { get; }
        Size Size { get; }
    }
}
