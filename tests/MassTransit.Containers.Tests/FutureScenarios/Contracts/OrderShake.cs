namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    public interface OrderShake :
        OrderLine
    {
        string Flavor { get; }
        Size Size { get; }
    }
}