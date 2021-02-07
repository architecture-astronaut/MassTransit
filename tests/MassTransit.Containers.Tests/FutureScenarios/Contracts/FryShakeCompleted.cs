namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface FryShakeCompleted :
        OrderLineCompleted
    {
        string Flavor { get; }
        Size Size { get; }
    }
}
