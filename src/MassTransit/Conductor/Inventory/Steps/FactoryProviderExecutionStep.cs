namespace MassTransit.Conductor.Inventory.Steps
{
    using System;
    using GreenPipes.Internals.Extensions;


    public class FactoryProviderExecutionStep<TInput, T, TResult> :
        IExecutionStep<TInput, T, TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        readonly AsyncMessageFactory<TInput, T> _factory;
        readonly IProviderRegistration<TInput, T> _providerRegistration;

        public FactoryProviderExecutionStep(IProviderRegistration<TInput, T> providerRegistration, AsyncMessageFactory<TInput, T> factory)
        {
            _providerRegistration = providerRegistration;
            _factory = factory;
        }

        public Type ServiceType => typeof(T);
        public Type RequestType => typeof(TInput);
        public Uri InputAddress => _providerRegistration.InputAddress;

        public void Build(BuildContext<TInput, TResult> context, IExecutionPlan<T, TResult> next)
        {
            NextBuildContext<T, TResult, TInput> nextContext = context.Create<T>();
            next.Build(nextContext);

            context.Factory(nextContext, _factory);
        }

        public override string ToString()
        {
            return $"Factory: {TypeCache<TInput>.ShortName} -> {TypeCache<T>.ShortName}";
        }
    }
}
