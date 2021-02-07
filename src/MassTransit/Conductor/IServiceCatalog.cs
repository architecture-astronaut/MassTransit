namespace MassTransit.Conductor
{
    using System;
    using Inventory;


    public interface IServiceCatalog
    {
        IExecutionPlanner<TResult> GetExecutionPlanner<TResult>(params Type[] inputTypes)
            where TResult : class;
    }
}
