namespace MassTransit
{
    using System.Threading.Tasks;
    using Conductor;
    using Conductor.Inventory.AsyncExecutor;


    public static class PlanExecutorExtensions
    {
        public static Task<TResult> Execute<TInput, TResult>(this IPlanExecutor<TInput, TResult> executor, ConsumeContext<TInput> context)
            where TResult : class
            where TInput : class
        {
            return executor.Execute(new ExecutePlanContext<TInput>(context, context.Message));
        }

        public static Task<TResult> Execute<TInput, TResult>(this IPlanExecutor<TInput, TResult> executor, ConsumeContext context,
            TInput request)
            where TResult : class
            where TInput : class
        {
            return executor.Execute(new ExecutePlanContext<TInput>(context, request));
        }
    }
}
