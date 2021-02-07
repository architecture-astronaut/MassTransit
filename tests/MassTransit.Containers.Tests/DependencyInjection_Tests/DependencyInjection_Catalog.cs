namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Conductor;
    using Conductor.Inventory;
    using ExtensionsDependencyInjectionIntegration;
    using Futures;
    using FutureScenarios.Contracts;
    using FutureScenarios.ItineraryPlanners;
    using FutureScenarios.Services;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_Catalog :
        Common_Catalog
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Catalog()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .AddGenericRequestClient()
                .AddSingleton<IFryer, Fryer>()
                .AddSingleton<IGrill, Grill>()
                .AddSingleton<IShakeMachine, ShakeMachine>()
                .AddScoped<IItineraryPlanner<OrderBurger>, BurgerItineraryPlanner>()
                .BuildServiceProvider(true);
        }

        protected override IServiceCatalog Catalog => _provider.GetRequiredService<IServiceCatalog>();
        protected override IAsyncPlanExecutorFactory ExecutorFactory => _provider.GetRequiredService<IAsyncPlanExecutorFactory>();
        protected override ServiceRegistry ServiceRegistry => _provider.GetRequiredService<Bind<IBus, ServiceRegistry>>().Value;

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();

        [OneTimeSetUp]
        public void ResolveTheBus()
        {
            _provider.GetRequiredService<IBus>();
        }

        protected override IRequestClient<T> GetRequestClient<T>()
        {
            var scope = _provider.CreateScope();

            return scope.ServiceProvider.GetRequiredService<IRequestClient<T>>();
        }
    }
}
