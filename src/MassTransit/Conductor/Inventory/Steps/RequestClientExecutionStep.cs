namespace MassTransit.Conductor.Inventory.Steps
{
    using System;
    using GreenPipes.Internals.Extensions;


    public class RequestClientExecutionStep<TRequest, TResponse, TResult> :
        IExecutionStep<TRequest, TResponse, TResult>
        where TRequest : class
        where TResponse : class
        where TResult : class
    {
        readonly IProviderRegistration<TRequest, TResponse> _providerRegistration;

        public RequestClientExecutionStep(IProviderRegistration<TRequest, TResponse> providerRegistration)
        {
            _providerRegistration = providerRegistration;
        }

        public Type ServiceType => typeof(TResponse);
        public Type RequestType => typeof(TRequest);
        public Uri InputAddress => _providerRegistration.InputAddress;

        public void Build(BuildContext<TRequest, TResult> context, IExecutionPlan<TResponse, TResult> next)
        {
            NextBuildContext<TResponse, TResult, TRequest> nextContext = context.Create<TResponse>();
            next.Build(nextContext);

            context.RequestEndpoint(nextContext, InputAddress);
        }

        public override string ToString()
        {
            return $"Request: {TypeCache<TRequest>.ShortName} -> {TypeCache<TResponse>.ShortName}";
        }
    }
}
