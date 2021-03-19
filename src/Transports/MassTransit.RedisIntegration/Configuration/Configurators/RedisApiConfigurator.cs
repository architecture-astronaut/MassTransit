namespace MassTransit.RedisIntegration.Configurators
{
    using System;


    public class RedisApiConfigurator :
        IRedisApiConfigurator
    {
        readonly ClientConfig _clientConfig;

        public RedisApiConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public bool? Request
        {
            set => _clientConfig.ApiVersionRequest = value;
        }

        public TimeSpan? RequestTimeout
        {
            set => _clientConfig.ApiVersionRequestTimeoutMs = value?.Milliseconds;
        }

        public TimeSpan? FallbackTimeout
        {
            set => _clientConfig.ApiVersionFallbackMs = value?.Milliseconds;
        }

        public string BrokerVersionFallback
        {
            set => _clientConfig.BrokerVersionFallback = value;
        }
    }
}
