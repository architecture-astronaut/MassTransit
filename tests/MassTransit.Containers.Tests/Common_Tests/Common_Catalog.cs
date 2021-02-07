namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using CatalogComponents;
    using CatalogContracts;
    using Conductor;
    using Conductor.Inventory;
    using FutureScenarios.Consumers;
    using FutureScenarios.Contracts;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class Common_Catalog :
        InMemoryTestFixture
    {
        protected abstract IServiceCatalog Catalog { get; }
        protected abstract IAsyncPlanExecutorFactory ExecutorFactory { get; }
        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract ServiceRegistry ServiceRegistry { get; }

        protected abstract IRequestClient<T> GetRequestClient<T>()
            where T : class;

        [Test]
        public async Task Should_complete_the_request()
        {
            IRequestClient<PrepareOrder> client = GetRequestClient<PrepareOrder>();

            using RequestHandle<PrepareOrder> requestHandle = client.Create(new
            {
                OrderId = NewId.NextGuid(),
                Fry = new
                {
                    FryId = NewId.NextGuid(),
                    Size = Size.Large,
                }
            });

            requestHandle.TimeToLive = default;

            Response<OrderReady> response = await requestHandle.GetResponse<OrderReady>();
        }

        [Test]
        public void Should_add_a_service_definition()
        {
            IPlanExecutor<PingMessage, PongMessage> planExecutor = ExecutorFactory.CreateExecutor<PingMessage, PongMessage>();
        }

        [Test]
        public void Should_add_a_two_step_service_definition()
        {
            IPlanExecutor<BlingMessage, PongMessage> planExecutor = ExecutorFactory.CreateExecutor<BlingMessage, PongMessage>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ServiceRegistry.Connect(configurator);

            configurator.ConfigureEndpoints(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumer<CookFryConsumer, CookFryConsumerDefinition>();
            configurator.AddConsumer<PrepareOrderConsumer, PrepareOrderConsumerDefinition>();

            configurator.AddConsumer<PingConsumer, PingConsumerDefinition>();
            configurator.AddConsumer<BlingConsumer, BlingConsumerDefinition>();


            configurator.AddBus(provider => BusControl);
        }
    }


    namespace CatalogContracts
    {
        using System;
        using FutureScenarios.Contracts;


        public interface PrepareOrder
        {
            Guid OrderId { get; }
            Fry Fry { get; }
        }


        public interface OrderReady
        {
            Guid OrderId { get; }
            FryCompleted Fry { get; }
        }
    }


    namespace CatalogComponents
    {
        using System;
        using CatalogContracts;
        using Conductor;
        using Conductor.Inventory.AsyncExecutor;
        using Definition;
        using FutureScenarios.Contracts;
        using TestFramework.Messages;


        class PingConsumerDefinition :
            ConsumerDefinition<PingConsumer>
        {
            public PingConsumerDefinition()
            {
                ConcurrentMessageLimit = 64;
            }

            public override void Configure(IServiceRegistry registry)
            {
                registry.AddStep<PingMessage, PongMessage>(x => x.Consumer<PingConsumer>(), definition =>
                {
                    definition.PartitionBy(x => x.CorrelationId);
                });
            }
        }


        class BlingConsumerDefinition :
            ConsumerDefinition<BlingConsumer>
        {
            public BlingConsumerDefinition()
            {
                ConcurrentMessageLimit = 64;
            }

            public override void Configure(IServiceRegistry registry)
            {
                registry.AddStep<BlingMessage, PingMessage>(x => x.Consumer<BlingConsumer>(), definition =>
                {
                    definition.PartitionBy(x => x.CorrelationId);
                });
            }
        }


        class BlingMessage
        {
            public Guid CorrelationId { get; set; }
        }


        class BlingConsumer :
            IConsumer<BlingMessage>
        {
            public Task Consume(ConsumeContext<BlingMessage> context)
            {
                return context.RespondAsync(new PingMessage(context.Message.CorrelationId));
            }
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        public class PrepareOrderConsumer :
            IConsumer<PrepareOrder>
        {
            readonly IPlanExecutor<Fry, FryCompleted> _planExecutor;

            public PrepareOrderConsumer(IAsyncPlanExecutorFactory asyncPlanExecutorFactory)
            {
                _planExecutor = asyncPlanExecutorFactory.CreateExecutor<Fry, FryCompleted>();
            }

            public async Task Consume(ConsumeContext<PrepareOrder> context)
            {
                var planContext = new ExecutePlanContext<PrepareOrder>(context, context.Message);

                var fryCompleted = await _planExecutor.Execute(planContext.Push(context.Message.Fry));

                await context.RespondAsync<OrderReady>(new
                {
                    context.Message.OrderId,
                    FryCompletd = fryCompleted
                });
            }
        }


        public class PrepareOrderConsumerDefinition :
            ConsumerDefinition<PrepareOrderConsumer>
        {
            public override void Configure(IServiceRegistry registry)
            {
                registry.AddStep<PrepareOrder, OrderReady>(x => x.Consumer<PrepareOrderConsumer>());

                registry.AddMessageInitializer<Fry, CookFry>(x => new
                {
                    x.Select<PrepareOrder>().OrderId,
                    OrderLineId = x.Data.FryId,
                    x.Data.Size
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
