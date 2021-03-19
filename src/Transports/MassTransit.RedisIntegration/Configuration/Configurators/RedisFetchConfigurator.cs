namespace MassTransit.RedisIntegration.Configurators
{
    using System;


    public class RedisFetchConfigurator :
        IRedisFetchConfigurator
    {
        readonly ConsumerConfig _consumerConfig;

        public RedisFetchConfigurator(ConsumerConfig consumerConfig)
        {
            _consumerConfig = consumerConfig;
        }

        public TimeSpan? WaitMaxInterval
        {
            set => _consumerConfig.FetchWaitMaxMs = value?.Milliseconds;
        }

        public int? MaxPartitionBytes
        {
            set => _consumerConfig.MaxPartitionFetchBytes = value;
        }

        public int? MaxBytes
        {
            set => _consumerConfig.FetchMaxBytes = value;
        }

        public int? MinBytes
        {
            set => _consumerConfig.FetchMinBytes = value;
        }

        public TimeSpan? ErrorBackoffInterval
        {
            set => _consumerConfig.FetchErrorBackoffMs = value?.Milliseconds;
        }
    }
}
