namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    using System;


    public interface ShakeReady
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }

        string Flavor { get; }
        Size Size { get; }
    }
}