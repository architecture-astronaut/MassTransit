namespace MassTransit.Conductor.Inventory.AsyncExecutor
{
    using System;
    using System.Threading.Tasks;
    using Context;


    public class ExecutePlanContext<TData> :
        ConsumeContextProxy,
        PlanContext<TData>
        where TData : class
    {
        ExecutePlanContext(ConsumeContext context)
            : base(context)
        {
        }

        ExecutePlanContext(PlanContext left, TData value)
            : base(left)
        {
            Left = left;
            Data = value;
        }

        public ExecutePlanContext(ConsumeContext context, TData value)
            : base(context)
        {
            Left = new ExecutePlanContext<TData>(context);
            Data = value;
        }

        public Task ForEach<T>(Func<PlanContext<T>, Task> callback)
            where T : class
        {
            if (this is PlanContext<T> stepContext)
            {
                async Task ForEachAsync()
                {
                    await callback(stepContext).ConfigureAwait(false);

                    if (Left != null)
                        await Left.ForEach(callback).ConfigureAwait(false);
                }

                return ForEachAsync();
            }


            return Left?.ForEach(callback) ?? Task.CompletedTask;
        }

        public T Select<T>()
            where T : class
        {
            if (Data is T result)
                return result;

            return Left?.Select<T>() ?? default(T);
        }

        public TData Data { get; }

        public PlanContext<T> Push<T>(T value)
            where T : class
        {
            return new ExecutePlanContext<T>(this, value);
        }

        public PlanContext Left { get; }
    }
}
