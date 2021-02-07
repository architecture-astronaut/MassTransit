namespace MassTransit.Conductor.Inventory
{
    using System;
    using Steps;


    public class RequestEndpointProviderRegistration<TInput, TResult> :
        IProviderRegistration<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        IReceiveEndpointConfigurator _receiveEndpointConfigurator;

        public Type ServiceType => typeof(TResult);
        public Type InputType => typeof(TInput);

        public Uri InputAddress => _receiveEndpointConfigurator?.InputAddress;

        public void OnConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _receiveEndpointConfigurator = configurator;
        }

        public IExecutionStep<TResponse> CreateResolutionStep<TResponse>()
            where TResponse : class
        {
            return new RequestClientExecutionStep<TInput, TResult, TResponse>(this);
        }
    }
}
