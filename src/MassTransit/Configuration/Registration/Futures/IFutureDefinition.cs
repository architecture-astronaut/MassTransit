namespace MassTransit.Registration.Futures
{
    using System;
    using Automatonymous;
    using Conductor.Inventory;
    using Definition;
    using MassTransit.Futures;


    public interface IFutureDefinition<TFuture> :
        IFutureDefinition
        where TFuture : MassTransitStateMachine<FutureState>
    {
        /// <summary>Sets the endpoint definition, if available</summary>
        new IEndpointDefinition<TFuture> EndpointDefinition { set; }

        /// <summary>Configure the future on the receive endpoint</summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="sagaConfigurator">The consumer configurator</param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FutureState> sagaConfigurator);

        /// <summary>
        /// Called by the <see cref="T:MassTransit.Definition.SagaMessageDefinition`2" /> to configure any saga-level definitions, such as message partitioning.
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="sagaMessageConfigurator"></param>
        /// <typeparam name="T"></typeparam>
        void Configure<T>(IReceiveEndpointConfigurator endpointConfigurator, ISagaMessageConfigurator<FutureState, T> sagaMessageConfigurator)
            where T : class;
    }


    public interface IFutureDefinition :
        IDefinition,
        IConfigureServiceRegistry
    {
        Type FutureType { get; }

        IEndpointDefinition EndpointDefinition { get; }

        /// <summary>
        /// Return the endpoint name for the future
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetEndpointName(IEndpointNameFormatter formatter);
    }
}
