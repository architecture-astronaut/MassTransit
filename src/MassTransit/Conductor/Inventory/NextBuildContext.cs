namespace MassTransit.Conductor.Inventory
{
    public interface NextBuildContext<TRequest, TResponse, TLeft> :
        BuildContext<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where TLeft : class
    {
        IPlanExecutor<TLeft, TResponse> BuildPath(IPlanStep<TLeft, TRequest, TResponse> step);
    }
}
