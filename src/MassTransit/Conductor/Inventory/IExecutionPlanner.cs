namespace MassTransit.Conductor.Inventory
{
    public interface IExecutionPlanner<TResult>
        where TResult : class
    {
        IExecutionPlan<TInput, TResult> BuildExecutionPlan<TInput>()
            where TInput : class;
    }
}
