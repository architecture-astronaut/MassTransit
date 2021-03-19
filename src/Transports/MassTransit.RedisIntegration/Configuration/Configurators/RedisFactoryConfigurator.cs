namespace MassTransit.RedisIntegration.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contexts;
    using GreenPipes;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Registration;
    using Serializers;
    using Specifications;
    using Transport;
    using Transports;
    using Util;


    public class RedisFactoryConfigurator :
        IRedisFactoryConfigurator,
        IRedisHostConfiguration
    {
        readonly ClientConfig _clientConfig;
        readonly Recycle<IClientContextSupervisor> _clientSupervisor;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly List<IRedisProducerSpecification> _producers;
        readonly SendObservable _sendObservers;
        readonly List<IRedisConsumerSpecification> _Streams;
        Action<ISendPipeConfigurator> _configureSend;
        IHeadersDeserializer _headersDeserializer;
        IHeadersSerializer _headersSerializer;
        bool _isHostConfigured;

        public RedisFactoryConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
            _Streams = new List<IRedisConsumerSpecification>();
            _producers = new List<IRedisProducerSpecification>();
            _endpointObservers = new ReceiveEndpointObservable();
            _sendObservers = new SendObservable();

            SetHeadersDeserializer(DictionaryHeadersSerialize.Deserializer);
            SetHeadersSerializer(DictionaryHeadersSerialize.Serializer);

            _clientSupervisor = new Recycle<IClientContextSupervisor>(() => new ClientContextSupervisor(_clientConfig, _producers));
        }

        public void Host(IReadOnlyList<string> servers, Action<IRedisHostConfigurator> configure)
        {
            if (servers == null || !servers.Any())
                throw new ArgumentException(nameof(servers));

            if (_isHostConfigured)
                throw new ConfigurationException("Host may not be specified more than once.");
            _isHostConfigured = true;

            const string serverSeparator = ",";

            _clientConfig.BootstrapServers = string.Join(serverSeparator, servers);
            var configurator = new RedisHostConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public void ConfigureApi(Action<IRedisApiConfigurator> configure)
        {
            var configurator = new RedisApiConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public void ConfigureSocket(Action<IRedisSocketConfigurator> configure)
        {
            var configurator = new RedisSocketConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public void StreamEndpoint<TKey, TValue>(string StreamName, string groupId, Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = CreateSpecification(StreamName, groupId, configure);
            _Streams.Add(specification);
        }

        public void StreamEndpoint<TKey, TValue>(string StreamName, ConsumerConfig consumerConfig,
            Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = CreateSpecification(StreamName, consumerConfig, configure);
            _Streams.Add(specification);
        }

        void IRedisFactoryConfigurator.StreamProducer<TKey, TValue>(string StreamName, Action<IRedisProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            this.StreamProducer(StreamName, new ProducerConfig(), configure);
        }

        void IRedisFactoryConfigurator.StreamProducer<TKey, TValue>(string StreamName, ProducerConfig producerConfig,
            Action<IRedisProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (string.IsNullOrWhiteSpace(StreamName))
                throw new ArgumentException(nameof(StreamName));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            var configurator = new RedisProducerSpecification<TKey, TValue>(this, producerConfig, StreamName, _headersSerializer);
            configure?.Invoke(configurator);

            configurator.ConnectSendObserver(_sendObservers);
            if (_configureSend != null)
                configurator.ConfigureSend(_configureSend);

            _producers.Add(configurator);
        }

        public void SetHeadersDeserializer(IHeadersDeserializer deserializer)
        {
            _headersDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Acks? Acks
        {
            set => _clientConfig.Acks = value;
        }

        public string ClientId
        {
            set => _clientConfig.ClientId = value;
        }

        public int? MessageMaxBytes
        {
            set => _clientConfig.MessageMaxBytes = value;
        }

        public int? MessageCopyMaxBytes
        {
            set => _clientConfig.MessageCopyMaxBytes = value;
        }

        public int? ReceiveMessageMaxBytes
        {
            set => _clientConfig.ReceiveMessageMaxBytes = value;
        }

        public int? MaxInFlight
        {
            set => _clientConfig.MaxInFlight = value;
        }

        public TimeSpan? MetadataRequestTimeout
        {
            set => _clientConfig.MetadataRequestTimeoutMs = value?.Milliseconds;
        }

        public TimeSpan? StreamMetadataRefreshInterval
        {
            set => _clientConfig.StreamMetadataRefreshIntervalMs = value?.Milliseconds;
        }

        public TimeSpan? MetadataMaxAge
        {
            set => _clientConfig.MetadataMaxAgeMs = value?.Milliseconds;
        }

        public TimeSpan? StreamMetadataRefreshFastInterval
        {
            set => _clientConfig.StreamMetadataRefreshFastIntervalMs = value?.Milliseconds;
        }

        public bool? StreamMetadataRefreshSparse
        {
            set => _clientConfig.StreamMetadataRefreshSparse = value;
        }

        public string StreamBlacklist
        {
            set => _clientConfig.StreamBlacklist = value;
        }

        public string Debug
        {
            set => _clientConfig.Debug = value;
        }

        public int? BrokerAddressTtl
        {
            set => _clientConfig.BrokerAddressTtl = value;
        }

        public BrokerAddressFamily? BrokerAddressFamily
        {
            set => _clientConfig.BrokerAddressFamily = value;
        }

        public TimeSpan? ReconnectBackoff
        {
            set => _clientConfig.ReconnectBackoffMs = value?.Milliseconds;
        }

        public TimeSpan? ReconnectBackoffMax
        {
            set => _clientConfig.ReconnectBackoffMaxMs = value?.Milliseconds;
        }

        public TimeSpan? StatisticsInterval
        {
            set => _clientConfig.StatisticsIntervalMs = value?.Milliseconds;
        }

        public bool? LogQueue
        {
            set => _clientConfig.LogQueue = value;
        }

        public bool? LogThreadName
        {
            set => _clientConfig.LogThreadName = value;
        }

        public bool? LogConnectionClose
        {
            set => _clientConfig.LogConnectionClose = value;
        }

        public int? InternalTerminationSignal
        {
            set => _clientConfig.InternalTerminationSignal = value;
        }

        public SecurityProtocol? SecurityProtocol
        {
            set => _clientConfig.SecurityProtocol = value;
        }

        public string PluginLibraryPaths
        {
            set => _clientConfig.PluginLibraryPaths = value;
        }

        public string ClientRack
        {
            set => _clientConfig.ClientRack = value;
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public IRedisConsumerSpecification CreateSpecification<TKey, TValue>(string StreamName, string groupId,
            Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentException(groupId);

            return CreateSpecification(StreamName, new ConsumerConfig {GroupId = groupId}, configure);
        }

        public IRedisConsumerSpecification CreateSpecification<TKey, TValue>(string StreamName, ConsumerConfig consumerConfig,
            Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (consumerConfig == null)
                throw new ArgumentNullException(nameof(consumerConfig));

            consumerConfig.AutoCommitIntervalMs = null;
            consumerConfig.EnableAutoCommit = false;

            var specification = new RedisConsumerSpecification<TKey, TValue>(this, consumerConfig, StreamName, _headersDeserializer, configure);
            specification.ConnectReceiveEndpointObserver(_endpointObservers);
            return specification;
        }

        public IReadOnlyDictionary<string, string> Configuration => _clientConfig.ToDictionary(x => x.Key, x => x.Value);

        public IClientContextSupervisor ClientContextSupervisor => _clientSupervisor.Supervisor;

        public IRedisRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
        {
            var endpoints = new ReceiveEndpointCollection();
            foreach (var Stream in _Streams)
                endpoints.Add(Stream.EndpointName, Stream.CreateReceiveEndpoint(busInstance));

            var StreamProducerProvider = new StreamProducerProvider(busInstance, this);

            return new RedisRider(this, busInstance, endpoints, StreamProducerProvider, context);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_clientConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");

            foreach (KeyValuePair<string, IRedisConsumerSpecification[]> kv in _Streams.GroupBy(x => x.EndpointName)
                .ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"Stream: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }

            foreach (var result in _producers.SelectMany(x => x.Validate()))
                yield return result;
        }

        public IBusInstanceSpecification Build(IRiderRegistrationContext context)
        {
            return new RedisBusInstanceSpecification(context, this);
        }
    }
}
