namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface ShakeCompleted :
        OrderLineCompleted
    {
        string Flavor { get; }
        Size Size { get; }
    }
}