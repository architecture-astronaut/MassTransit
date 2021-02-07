namespace MassTransit.Conductor
{
    using System.Threading.Tasks;


    public interface IPlanExecutor<in TInput, TResult>
        where TResult : class
        where TInput : class
    {
        Task<TResult> Execute(PlanContext<TInput> plan);
    }
}
