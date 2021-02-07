namespace MassTransit.Conductor.Inventory
{
    using System;


    public interface IExecutionStep<TInput, T, TResult> :
        IExecutionStep<TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        void Build(BuildContext<TInput, TResult> context, IExecutionPlan<T, TResult> next);
    }


    public interface IExecutionStep<TResult>
        where TResult : class
    {
        Type ServiceType { get; }
        Type RequestType { get; }
        Uri InputAddress { get; }
    }
}
