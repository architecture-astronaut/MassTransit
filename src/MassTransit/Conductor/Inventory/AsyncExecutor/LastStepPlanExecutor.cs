namespace MassTransit.Conductor.Inventory.AsyncExecutor
{
    using System.Threading.Tasks;


    public class LastStepPlanExecutor<TInput, TResult> :
        IPlanExecutor<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        readonly IPlanStep<TInput, TResult, TResult> _step;

        public LastStepPlanExecutor(IPlanStep<TInput, TResult, TResult> step)
        {
            _step = step;
        }

        public Task<TResult> Execute(PlanContext<TInput> plan)
        {
            return _step.Execute(plan, Cache.LastStep);
        }


        static class Cache
        {
            internal static readonly IPlanExecutor<TResult, TResult> LastStep = new Response();
        }


        class Response :
            IPlanExecutor<TResult, TResult>
        {
            public Task<TResult> Execute(PlanContext<TResult> plan)
            {
                return Task.FromResult(plan.Data);
            }
        }
    }
}
