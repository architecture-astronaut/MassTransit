namespace MassTransit.RedisIntegration.Configurators
{
    using System;
    using StackExchange.Redis;


    public class RedisHostConfigurator :
        IRedisHostConfigurator
    {
        readonly ConfigurationOptions _clientConfig;

        public RedisHostConfigurator(ConfigurationOptions clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public void UseSsl(Action<IRedisSslConfigurator> configure)
        {
            var configurator = new RedisSslConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public void UseSasl(Action<IRedisSaslConfigurator> configure)
        {
            var configurator = new RedisSaslConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        //public void CancellationDelay(TimeSpan timeSpan)
        //{
        //    _clientConfig.CancellationDelayMaxMs = timeSpan.Milliseconds;
        //}
    }
}
