namespace MassTransit.Conductor.Inventory.AsyncExecutor
{
    using System.Threading.Tasks;


    public class NextStepPlanExecutor<TInput, T, TResult> :
        IPlanExecutor<TInput, TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        readonly IPlanExecutor<T, TResult> _next;
        readonly IPlanStep<TInput, T, TResult> _step;

        public NextStepPlanExecutor(IPlanStep<TInput, T, TResult> step, IPlanExecutor<T, TResult> next)
        {
            _step = step;
            _next = next;
        }

        public Task<TResult> Execute(PlanContext<TInput> plan)
        {
            return _step.Execute(plan, _next);
        }
    }
}
