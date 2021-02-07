namespace MassTransit.Conductor.Inventory
{
    using System.Threading.Tasks;


    public interface IPlanStep<in TInput, out T, TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        Task<TResult> Execute(PlanContext<TInput> context, IPlanExecutor<T, TResult> next);
    }
}
