namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    using System;


    public interface FryReady
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        Size Size { get; }
    }
}