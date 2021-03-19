namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using MassTransit.Registration;


    public class StreamProducerProvider :
        IStreamProducerProvider
    {
        readonly IBusInstance _busInstance;
        readonly IRedisHostConfiguration _hostConfiguration;

        public StreamProducerProvider(IBusInstance busInstance, IRedisHostConfiguration hostConfiguration)
        {
            _busInstance = busInstance;
            _hostConfiguration = hostConfiguration;
        }

        public IStreamProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class
        {
            return _hostConfiguration.ClientContextSupervisor.CreateProducer<TKey, TValue>(_busInstance, address);
        }
    }
}
