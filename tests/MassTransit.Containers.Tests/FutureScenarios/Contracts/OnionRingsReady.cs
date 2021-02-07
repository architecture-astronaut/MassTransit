namespace MassTransit.Containers.Tests.FutureScenarios.Contracts
{
    using System;


    public interface OnionRingsReady
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        int Quantity { get; }
    }
}