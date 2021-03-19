namespace MassTransit.RedisIntegration.Activities
{
    using Automatonymous.Activities;
    using GreenPipes;
    using Metadata;


    static class ProducerFactoryExtensions
    {
        internal static IStreamProducer<T> GetProducer<T>(this PipeContext context)
            where T : class
        {
            var factory = context.GetStateMachineActivityFactory();

            IStreamProducer<T> producer = factory.GetService<IStreamProducer<T>>(context) ??
                throw new ProduceException($"StreamProducer<{TypeMetadataCache<T>.ShortName} not found");

            return producer;
        }
    }
}
