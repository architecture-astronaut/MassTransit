namespace MassTransit.Conductor.Inventory.AsyncExecutor
{
    using System;
    using Context;


    public class AsyncPlanExecutorFactory :
        IAsyncPlanExecutorFactory
    {
        readonly IServiceCatalog _catalog;

        public AsyncPlanExecutorFactory(IServiceCatalog catalog)
        {
            _catalog = catalog;
        }

        public IPlanExecutor<TInput, TResult> CreateExecutor<TInput, TResult>()
            where TInput : class
            where TResult : class
        {
            IExecutionPlanner<TResult> planner = _catalog.GetExecutionPlanner<TResult>(typeof(TInput));

            IExecutionPlan<TInput, TResult> plan = planner.BuildExecutionPlan<TInput>();

            LogContext.Debug?.Log("Building Executor" + Environment.NewLine + "{Plan}", plan.ToString());

            var builder = new AsyncPlanBuilder<TInput, TResult>();

            plan.Build(builder);

            return builder.GetExecutor();
        }
    }
}
