namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OnionRingsCompleted :
        OrderLineCompleted
    {
        int Quantity { get; }
    }
}