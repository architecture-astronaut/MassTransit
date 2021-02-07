namespace MassTransit.Futures
{
    using System.Threading.Tasks;


    public static class FutureVariableExtensions
    {
        public static async Task<TValue> SetVariable<T, TValue>(this FutureConsumeContext<T> context, string key, AsyncFutureMessageFactory<T, TValue> factory)
            where T : class
            where TValue : class
        {
            var value = await factory(context).ConfigureAwait(false);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static async Task<TValue> SetVariable<TValue>(this FutureConsumeContext context, string key, AsyncFutureMessageFactory<TValue> factory)
            where TValue : class
        {
            var value = await factory(context).ConfigureAwait(false);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static TValue SetVariable<T, TValue>(this FutureConsumeContext<T> context, string key, FutureMessageFactory<T, TValue> factory)
            where T : class
            where TValue : class
        {
            var value = factory(context);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static TValue SetVariable<TValue>(this FutureConsumeContext context, string key, FutureMessageFactory<TValue> factory)
            where TValue : class
        {
            var value = factory(context);

            context.Instance.Variables[key] = value;

            return value;
        }

        public static void SetVariable<TValue>(this FutureConsumeContext context, string key, TValue value)
            where TValue : class
        {
            context.Instance.Variables[key] = value;
        }

        public static bool TryGetVariable<T>(this FutureConsumeContext future, string key, out T result)
            where T : class
        {
            if (future.Instance.HasVariables())
                return future.Instance.Variables.TryGetValue(key, out result);

            result = default;
            return false;
        }
    }
}
