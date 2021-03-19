namespace MassTransit.RedisIntegration.Configurators
{
    using StackExchange.Redis;


    public class RedisSslConfigurator :
        IRedisSslConfigurator
    {
        readonly ConfigurationOptions _clientConfig;

        public RedisSslConfigurator(ConfigurationOptions clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public bool UseSsl
        {
            set => _clientConfig.Ssl = value;
        }

        public string SslHost
        {
            set => _clientConfig.SslHost = value;
        }

        public System.Security.Authentication.SslProtocols SslProtocols
        {
            set => _clientConfig.SslProtocols = value;
        }
    }
}
