namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderOnionRings :
        OrderLine
    {
        int Quantity { get; }
    }
}