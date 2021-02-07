namespace MassTransit.Registration.Sagas
{
    public class InMemorySagaRepositoryRegistrationProvider :
        SagaRepositoryRegistrationProvider
    {
        public override void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
        {
            configurator.InMemoryRepository();
        }
    }
}
