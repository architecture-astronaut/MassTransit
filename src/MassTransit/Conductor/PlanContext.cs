namespace MassTransit.Conductor
{
    using System;
    using System.Threading.Tasks;


    public interface PlanContext<out TData> :
        PlanContext
    {
        TData Data { get; }

        PlanContext<T> Push<T>(T value)
            where T : class;
    }


    public interface PlanContext :
        ConsumeContext
    {
        PlanContext Left { get; }

        public Task ForEach<T>(Func<PlanContext<T>, Task> callback)
            where T : class;

        public T Select<T>()
            where T : class;
    }
}
