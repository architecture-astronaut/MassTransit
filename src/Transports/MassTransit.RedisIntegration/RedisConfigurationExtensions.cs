namespace MassTransit.RedisIntegration
{
    using System.Collections.Generic;
    using System.Linq;


    public static class RedisConfigurationExtensions
    {
        public static ConsumerConfig GetConsumerConfig(this IRedisHostConfiguration hostConfiguration, ConsumerConfig config)
        {
            Dictionary<string, string> settings = hostConfiguration.Configuration.ToDictionary(setting => setting.Key, setting => setting.Value);

            foreach (KeyValuePair<string, string> setting in config)
                settings[setting.Key] = setting.Value;

            return new ConsumerConfig(settings);
        }

        public static ProducerConfig GetProducerConfig(this IRedisHostConfiguration hostConfiguration, ProducerConfig config)
        {
            Dictionary<string, string> settings = hostConfiguration.Configuration.ToDictionary(setting => setting.Key, setting => setting.Value);

            foreach (KeyValuePair<string, string> setting in config)
                settings[setting.Key] = setting.Value;

            return new ProducerConfig(settings);
        }
    }
}
