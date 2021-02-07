namespace MassTransit.Conductor
{
    public interface IAsyncPlanExecutorFactory
    {
        IPlanExecutor<TInput, TResult> CreateExecutor<TInput, TResult>()
            where TInput : class
            where TResult : class;
    }
}
