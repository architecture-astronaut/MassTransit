namespace MassTransit.Conductor
{
    using System;
    using Inventory;


    public interface IServiceRegistry
    {
        /// <summary>
        /// Add a service to the registry, specifying the service type and the provider input type
        /// </summary>
        /// <param name="providerSelector"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        void AddStep<TInput, TResult>(Func<IServiceProviderSelector<TInput, TResult>, IServiceProviderDefinition<TInput, TResult>> providerSelector,
            Action<IServiceRegistrationConfigurator<TInput>> configure = default)
            where TResult : class
            where TInput : class;
    }
}
