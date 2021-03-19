namespace MassTransit.RedisIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using GreenPipes;
    using MassTransit.Configurators;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Registration;
    using Serializers;
    using StackExchange.Redis;
    using Transports;


    public class RedisConsumerSpecification<TKey, TValue> :
        IRedisConsumerSpecification
        where TValue : class
    {
        readonly Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> _configure;
        readonly ConfigurationOptions _consumerConfig;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IRedisHostConfiguration _hostConfiguration;
        readonly string _StreamName;

        public RedisConsumerSpecification(IRedisHostConfiguration hostConfiguration, ConfigurationOptions consumerConfig, string StreamName,
            IHeadersDeserializer headersDeserializer,
            Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
        {
            _hostConfiguration = hostConfiguration;
            _consumerConfig = consumerConfig;
            _StreamName = StreamName;
            _endpointObservers = new ReceiveEndpointObservable();
            _headersDeserializer = headersDeserializer;
            _configure = configure;
            EndpointName = $"{RedisStreamAddress.PathPrefix}/{_StreamName}";
        }

        public string EndpointName { get; }

        public ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance)
        {
            var endpointConfiguration = busInstance.HostConfiguration.CreateReceiveEndpointConfiguration(EndpointName);
            endpointConfiguration.ConnectReceiveEndpointObserver(_endpointObservers);

            var configurator =
                new RedisStreamReceiveEndpointConfiguration<TKey, TValue>(_hostConfiguration, _consumerConfig, _StreamName, busInstance, endpointConfiguration,
                    _headersDeserializer);
            _configure?.Invoke(configurator);

            var result = BusConfigurationResult.CompileResults(configurator.Validate());

            try
            {
                return configurator.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the Redis receive endpoint", ex);
            }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_StreamName))
                yield return this.Failure("Stream", "should not be empty");

            if (string.IsNullOrEmpty(_consumerConfig.GroupId))
                yield return this.Failure("GroupId", "should not be empty");
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }
    }
}
