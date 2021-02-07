namespace MassTransit.Conductor.Inventory.Steps
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Internals.Extensions;


    public class FactoryPlanStep<TInput, T, TResult> :
        IPlanStep<TInput, T, TResult>
        where TInput : class
        where TResult : class
        where T : class
    {
        readonly AsyncMessageFactory<TInput, T> _factory;

        public FactoryPlanStep(AsyncMessageFactory<TInput, T> factory)
        {
            _factory = factory;
        }

        public async Task<TResult> Execute(PlanContext<TInput> context, IPlanExecutor<T, TResult> next)
        {
            LogContext.Debug?.Log("Factory<{RequestType}, {ResultType}", TypeCache<TInput>.ShortName, TypeCache<T>.ShortName);

            var value = await _factory(context).ConfigureAwait(false);

            PlanContext<T> nextContext = context.Push(value);

            return await next.Execute(nextContext).ConfigureAwait(false);
        }
    }
}
