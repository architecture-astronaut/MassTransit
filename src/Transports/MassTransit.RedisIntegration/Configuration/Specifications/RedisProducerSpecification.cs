namespace MassTransit.RedisIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Registration;
    using Serializers;
    using Transport;


    public class RedisProducerSpecification<TKey, TValue> :
        IRedisProducerSpecification,
        IRedisProducerConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly IRedisHostConfiguration _hostConfiguration;
        readonly ProducerConfig _producerConfig;
        readonly SendObservable _sendObservers;
        Action<ISendPipeConfigurator> _configureSend;
        IHeadersSerializer _headersSerializer;
        ISerializer<TKey> _keySerializer;
        ISerializer<TValue> _valueSerializer;

        public RedisProducerSpecification(IRedisHostConfiguration hostConfiguration, ProducerConfig producerConfig, string StreamName,
            IHeadersSerializer headersSerializer)
        {
            _hostConfiguration = hostConfiguration;
            _producerConfig = producerConfig;
            StreamName = StreamName;
            _headersSerializer = headersSerializer;
            _sendObservers = new SendObservable();

            SetKeySerializer(new MassTransitJsonSerializer<TKey>());
            SetValueSerializer(new MassTransitJsonSerializer<TValue>());
            SetHeadersSerializer(headersSerializer);
        }

        public int? BatchNumMessages
        {
            set => _producerConfig.BatchNumMessages = value;
        }

        public CompressionType? CompressionType
        {
            set => _producerConfig.CompressionType = value;
        }

        public int? CompressionLevel
        {
            set => _producerConfig.CompressionLevel = value;
        }

        public int? QueueBufferingBackpressureThreshold
        {
            set => _producerConfig.QueueBufferingBackpressureThreshold = value;
        }

        public TimeSpan? RetryBackoff
        {
            set => _producerConfig.RetryBackoffMs = value == null ? (int?)null : Convert.ToInt32(value.Value.TotalMilliseconds);
        }

        public int? MessageSendMaxRetries
        {
            set => _producerConfig.MessageSendMaxRetries = value;
        }

        public TimeSpan? Linger
        {
            set => _producerConfig.LingerMs = value == null ? (int?)null : Convert.ToInt32(value.Value.TotalMilliseconds);
        }

        public int? QueueBufferingMaxKbytes
        {
            set => _producerConfig.QueueBufferingMaxKbytes = value;
        }

        public int? QueueBufferingMaxMessages
        {
            set => _producerConfig.QueueBufferingMaxMessages = value;
        }

        public bool? EnableGaplessGuarantee
        {
            set => _producerConfig.EnableGaplessGuarantee = value;
        }

        public bool? EnableIdempotence
        {
            set => _producerConfig.EnableIdempotence = value;
        }

        public TimeSpan? TransactionTimeout
        {
            set => _producerConfig.TransactionTimeoutMs = value == null ? (int?)null : Convert.ToInt32(value.Value.TotalMilliseconds);
        }

        public string TransactionalId
        {
            set => _producerConfig.TransactionalId = value;
        }
        
        public TimeSpan? MessageTimeout
        {
            set => _producerConfig.MessageTimeoutMs = value == null ? (int?)null : Convert.ToInt32(value.Value.TotalMilliseconds);
        }

        public TimeSpan? RequestTimeout
        {
            set => _producerConfig.RequestTimeoutMs = value == null ? (int?)null : Convert.ToInt32(value.Value.TotalMilliseconds);
        }

        public string DeliveryReportFields
        {
            set => _producerConfig.DeliveryReportFields = value;
        }

        public bool? EnableDeliveryReports
        {
            set => _producerConfig.EnableDeliveryReports = value;
        }

        public bool? EnableBackgroundPoll
        {
            set => _producerConfig.EnableBackgroundPoll = value;
        }

        public void SetKeySerializer(ISerializer<TKey> serializer)
        {
            _keySerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetValueSerializer(ISerializer<TValue> serializer)
        {
            _valueSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public string StreamName { get; }

        public IRedisProducerFactory CreateProducerFactory(IBusInstance busInstance)
        {
            var producerConfig = _hostConfiguration.GetProducerConfig(_producerConfig);
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.HostTopology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);

            ProducerBuilder<TKey, TValue> CreateProducerBuilder()
            {
                ProducerBuilder<TKey, TValue> producerBuilder = new ProducerBuilder<TKey, TValue>(producerConfig)
                    .SetErrorHandler((c, error) =>
                        busInstance.HostConfiguration.SendLogContext?.Error?.Log("Consumer error ({code}): {reason} on {Stream}", error.Code, error.Reason,
                            StreamName))
                    .SetLogHandler((c, message) => busInstance.HostConfiguration.SendLogContext?.Debug?.Log(message.Message));

                if (_keySerializer != null)
                    producerBuilder.SetKeySerializer(_keySerializer);
                if (_valueSerializer != null)
                    producerBuilder.SetValueSerializer(_valueSerializer);
                return producerBuilder;
            }

            var sendPipe = sendConfiguration.CreatePipe();

            return new ProducerContextSupervisor<TKey, TValue>(StreamName, sendPipe, _sendObservers, _hostConfiguration.ClientContextSupervisor,
                busInstance.HostConfiguration, _headersSerializer, CreateProducerBuilder);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(StreamName))
                yield return this.Failure("Stream", "should not be empty");
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
        }
    }
}
