namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    using System;
    using Topology;


    [ExcludeFromTopology]
    public interface OrderLine
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
    }
}