namespace MassTransit.Containers.Tests.FutureScenarios.Consumers
{
    using Conductor;
    using Contracts;
    using Definition;


    public class CookFryConsumerDefinition :
        ConsumerDefinition<CookFryConsumer>
    {
        public CookFryConsumerDefinition()
        {
            ConcurrentMessageLimit = 32;
        }

        public override void Configure(IServiceRegistry registry)
        {
            registry.AddStep<CookFry, FryReady>(x => x.Consumer<CookFryConsumer>());
        }
    }
}
