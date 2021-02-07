namespace MassTransit.Conductor.Inventory.AsyncExecutor
{
    using System;
    using GreenPipes.Internals.Extensions;
    using Steps;


    public class AsyncPlanBuilder<TInput, TResult> :
        BuildContext<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        protected IPlanExecutor<TInput, TResult> PlanExecutor { get; private set; }

        public void RequestEndpoint<T>(NextBuildContext<T, TResult, TInput> context, Uri inputAddress)
            where T : class
        {
            var step = new RequestClientPlanStep<TInput, T, TResult>(inputAddress);

            PlanExecutor = context.BuildPath(step);
        }

        public void Factory<T>(NextBuildContext<T, TResult, TInput> context, AsyncMessageFactory<TInput, T> factoryMethod)
            where T : class
        {
            var step = new FactoryPlanStep<TInput, T, TResult>(factoryMethod);

            PlanExecutor = context.BuildPath(step);
        }

        public void Result()
        {
            PlanExecutor = new ResultPlanExecutor<TInput, TResult>();
        }

        public NextBuildContext<T, TResult, TInput> Create<T>()
            where T : class
        {
            return new AsyncPlanBuilder<T, TResult, TInput>(this);
        }

        public IPlanExecutor<TInput, TResult> GetExecutor()
        {
            if (PlanExecutor == null)
                throw new InvalidOperationException("No build path configured");

            return PlanExecutor;
        }
    }


    public class AsyncPlanBuilder<TInput, TResult, TLeft> :
        AsyncPlanBuilder<TInput, TResult>,
        NextBuildContext<TInput, TResult, TLeft>
        where TResult : class
        where TInput : class
        where TLeft : class
    {
        readonly AsyncPlanBuilder<TLeft, TResult> _left;

        public AsyncPlanBuilder(AsyncPlanBuilder<TLeft, TResult> left)
        {
            _left = left;
        }

        public IPlanExecutor<TLeft, TResult> BuildPath(IPlanStep<TLeft, TInput, TResult> step)
        {
            if (PlanExecutor != null)
                return new NextStepPlanExecutor<TLeft, TInput, TResult>(step, PlanExecutor);

            if (step is IPlanStep<TLeft, TResult, TResult> lastStep)
                return new LastStepPlanExecutor<TLeft, TResult>(lastStep);

            throw new InvalidOperationException($"Last step type mismatch, expected {TypeCache<TResult>.ShortName}, eas {TypeCache<TInput>.ShortName}");
        }
    }
}
