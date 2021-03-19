namespace MassTransit.RedisIntegration
{
    using System;
    using Builders;
    using Configuration;
    using MassTransit.Registration;
    using Serializers;


    public class RedisReceiveEndpointBuilder<TKey, TValue> :
        ReceiveEndpointBuilder
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly Func<ConsumerBuilder<TKey, TValue>> _consumerBuilderFactory;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IRedisHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public RedisReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration,
            IRedisHostConfiguration hostConfiguration, ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
            _consumerBuilderFactory = consumerBuilderFactory;
        }

        public IRedisReceiveEndpointContext<TKey, TValue> CreateReceiveEndpointContext()
        {
            var context = new RedisReceiveEndpointContext<TKey, TValue>(_busInstance, _configuration, _hostConfiguration, _receiveSettings,
                _headersDeserializer, _consumerBuilderFactory);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
