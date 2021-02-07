namespace MassTransit.Conductor
{
    using System;
    using Inventory;


    public static class ServiceRegistryExtensions
    {
        public static void AddMessageInitializer<TInput, TResult>(this IServiceRegistry registry, ResultValueProvider<TInput> provider,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TInput : class
            where TResult : class
        {
            registry.AddStep<TInput, TResult>(x => x.Initializer(provider), configure);
        }

        public static void AddMessageFactory<TInput, TResult>(this IServiceRegistry registry, AsyncMessageFactory<TInput, TResult> factory,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TInput : class
            where TResult : class
        {
            registry.AddStep<TInput, TResult>(x => x.Factory(factory), configure);
        }

        public static void AddMessageFactory<TInput, TResult>(this IServiceRegistry registry, MessageFactory<TInput, TResult> factory,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TInput : class
            where TResult : class
        {
            registry.AddStep<TInput, TResult>(x => x.Factory(factory), configure);
        }
    }
}
