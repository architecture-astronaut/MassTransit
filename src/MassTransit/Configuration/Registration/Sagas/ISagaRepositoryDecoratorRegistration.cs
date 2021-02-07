namespace MassTransit.Registration.Sagas
{
    using MassTransit.Saga;


    public interface ISagaRepositoryDecoratorRegistration<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Decorate the container-based saga repository, returning the saga repository that should be
        /// used for receive endpoint registration
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        ISagaRepository<TSaga> DecorateSagaRepository(ISagaRepository<TSaga> repository);
    }
}
