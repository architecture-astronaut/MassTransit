namespace MassTransit.Conductor.Inventory
{
    using System;


    public class NextExecutionPlan<TRequest, T, TResponse> :
        IExecutionPlan<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where T : class
    {
        readonly IExecutionPlan<T, TResponse> _next;
        readonly IExecutionStep<TRequest, T, TResponse> _step;

        public NextExecutionPlan(IExecutionStep<TRequest, T, TResponse> step, IExecutionPlan<T, TResponse> next)
        {
            _step = step;
            _next = next;
        }

        public void Build(BuildContext<TRequest, TResponse> context)
        {
            _step.Build(context, _next);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _step.ToString(), _next.ToString());
        }
    }
}
