namespace MassTransit.RedisIntegration.Registration
{
    using System;
    using MassTransit.Registration;


    public class RedisProducerRegistrationConfigurator<TKey, TValue> :
        IRedisProducerRegistration
        where TValue : class
    {
        readonly Action<IRiderRegistrationContext, IRedisProducerConfigurator<TKey, TValue>> _configure;
        readonly ProducerConfig _producerConfig;
        readonly string _Stream;

        public RedisProducerRegistrationConfigurator(string Stream, Action<IRiderRegistrationContext, IRedisProducerConfigurator<TKey, TValue>> configure,
            ProducerConfig producerConfig = null)
        {
            _Stream = Stream;
            _producerConfig = producerConfig;
            _configure = configure;
        }

        public void Register(IRedisFactoryConfigurator configurator, IRiderRegistrationContext context)
        {
            if (_producerConfig != null)
                configurator.StreamProducer<TKey, TValue>(_Stream, _producerConfig, c => _configure?.Invoke(context, c));
            else
                configurator.StreamProducer<TKey, TValue>(_Stream, c => _configure?.Invoke(context, c));
        }
    }
}
