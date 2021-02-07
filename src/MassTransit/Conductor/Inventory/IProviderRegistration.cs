namespace MassTransit.Conductor.Inventory
{
    using System;
    using ConsumeConfigurators;


    public interface IProviderRegistration<TInput, TResult> :
        IProviderRegistration<TResult>
        where TResult : class
        where TInput : class
    {
    }


    public interface IProviderRegistration<TResult> :
        IProviderRegistration
        where TResult : class
    {
    }


    public interface IProviderRegistration
    {
        Type ServiceType { get; }
        Type InputType { get; }
        Uri InputAddress { get; }

        void OnConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator);

        IExecutionStep<TResponse> CreateResolutionStep<TResponse>()
            where TResponse : class;
    }
}
