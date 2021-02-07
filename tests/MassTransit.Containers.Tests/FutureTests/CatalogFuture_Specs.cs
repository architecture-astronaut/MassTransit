namespace MassTransit.Containers.Tests.FutureTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Conductor;
    using Conductor.Inventory.AsyncExecutor;
    using Definition;
    using ExtensionsDependencyInjectionIntegration;
    using Futures;
    using FutureScenarios.Activities;
    using FutureScenarios.Consumers;
    using FutureScenarios.Contracts;
    using FutureScenarios.Futures;
    using FutureScenarios.ItineraryPlanners;
    using FutureScenarios.Services;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Registration.Futures;


    public class Ordering_using_the_request_client :
        FutureTestFixture
    {
        [Test]
        public async Task Should_complete_the_request_using_futures()
        {
            using var scope = Provider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRequestClient<SubmitOrder>>();

            var orderId = NewId.NextGuid();
            var fryId = NewId.NextGuid();
            var shakeId = NewId.NextGuid();
            var burgerId = NewId.NextGuid();

            using RequestHandle<SubmitOrder> requestHandle = client.Create(new
            {
                OrderId = orderId,
                Fries = new[]
                {
                    new
                    {
                        FryId = fryId,
                        Size = Size.Large
                    }
                },
                Burgers = new[]
                {
                    new Burger
                    {
                        BurgerId = burgerId,
                        Weight = 1.0m,
                        Cheese = true
                    }
                },
                Shakes = new[]
                {
                    new
                    {
                        ShakeId = shakeId,
                        Size = Size.Large,
                        Flavor = "Chocolate"
                    }
                },
                FryShakes = default(FryShake[])
            });

            requestHandle.TimeToLive = default;

            Response<OrderCompleted> response = await requestHandle.GetResponse<OrderCompleted>();
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton<IGrill, Grill>();
            collection.AddScoped<IItineraryPlanner<OrderBurger>, BurgerItineraryPlanner>();
            collection.AddSingleton<IFryer, Fryer>();
            collection.AddSingleton<IShakeMachine, ShakeMachine>();
        }

        protected override void ConfigureMassTransit(IServiceCollectionBusConfigurator configurator)
        {
            configurator.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>();

            configurator.AddConsumer<CookFryConsumer, CookFryConsumerDefinition>();
            configurator.AddConsumer<PourShakeConsumer>();
            configurator.AddConsumer<CookOnionRingsConsumer>();

            configurator.AddFuture<BurgerFuture>(typeof(DefaultFutureDefinition<BurgerFuture>));
            configurator.AddFuture<FryFuture>(typeof(DefaultFutureDefinition<FryFuture>));
            configurator.AddFuture<ShakeFuture>(typeof(DefaultFutureDefinition<ShakeFuture>));

            configurator.AddActivitiesFromNamespaceContaining<GrillBurgerActivity>();
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            readonly IPlanExecutor<Burger, BurgerCompleted> _burgerExecutor;
            readonly IPlanExecutor<Fry, FryCompleted> _fryExecutor;
            readonly IPlanExecutor<Shake, ShakeCompleted> _shakeExecutor;

            public SubmitOrderConsumer(IAsyncPlanExecutorFactory asyncPlanExecutorFactory)
            {
                _fryExecutor = asyncPlanExecutorFactory.CreateExecutor<Fry, FryCompleted>();
                _burgerExecutor = asyncPlanExecutorFactory.CreateExecutor<Burger, BurgerCompleted>();
                _shakeExecutor = asyncPlanExecutorFactory.CreateExecutor<Shake, ShakeCompleted>();
            }

            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
                var planContext = new ExecutePlanContext<SubmitOrder>(context, context.Message);

                Task<FryCompleted[]> fries = Task.WhenAll(context.Message.Fries?.Select(x => _fryExecutor.Execute(planContext.Push(x)))
                    ?? Enumerable.Empty<Task<FryCompleted>>());
                Task<BurgerCompleted[]> burgers = Task.WhenAll(context.Message.Burgers?.Select(x => _burgerExecutor.Execute(planContext.Push(x)))
                    ?? Enumerable.Empty<Task<BurgerCompleted>>());
                Task<ShakeCompleted[]> shakes = Task.WhenAll(context.Message.Shakes?.Select(x => _shakeExecutor.Execute(planContext.Push(x)))
                    ?? Enumerable.Empty<Task<ShakeCompleted>>());

                await Task.WhenAll(fries, burgers, shakes);

                IEnumerable<OrderLineCompleted> completed = (await fries).Concat((await burgers).Concat((await shakes).Cast<OrderLineCompleted>()));

                await context.RespondAsync<OrderCompleted>(new
                {
                    context.Message.OrderId,
                    Created = context.SentTime ?? DateTime.UtcNow,
                    Completed = DateTime.UtcNow,
                    LinesCompleted = completed.ToDictionary(x => x.OrderLineId)
                });
            }
        }


        class SubmitOrderConsumerDefinition :
            ConsumerDefinition<SubmitOrderConsumer>
        {
            public override void Configure(IServiceRegistry registry)
            {
                registry.AddStep<SubmitOrder, OrderCompleted>(x => x.Consumer<SubmitOrderConsumer>());

                registry.AddMessageInitializer<Fry, OrderFry>(x => new
                {
                    x.Select<SubmitOrder>().OrderId,
                    OrderLineId = x.Data.FryId,
                    x.Data.Size
                });

                registry.AddMessageInitializer<Burger, OrderBurger>(x => new
                {
                    x.Select<SubmitOrder>().OrderId,
                    OrderLineId = x.Data.BurgerId,
                    Burger = x.Data
                });

                registry.AddMessageInitializer<Shake, OrderShake>(x => new
                {
                    x.Select<SubmitOrder>().OrderId,
                    OrderLineId = x.Data.ShakeId,
                    x.Data.Size,
                    x.Data.Flavor
                });

                registry.AddMessageInitializer<FryReady, FryCompleted>(x => new
                {
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow,
                    x.Data.OrderId,
                    x.Data.OrderLineId,
                    x.Data.Size,
                    Description = $"{x.Data.Size} Fries"
                });
            }
        }
    }
}
