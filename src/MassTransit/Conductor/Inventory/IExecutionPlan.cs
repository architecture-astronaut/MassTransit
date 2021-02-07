namespace MassTransit.Conductor.Inventory
{
    public interface IExecutionPlan<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        void Build(BuildContext<TInput, TResult> context);
    }
}
