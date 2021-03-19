namespace MassTransit.RedisIntegration
{
    using System;
    using MassTransit.Registration;
    using Registration;
    using Scoping;
    using Transport;


    public static class RedisProducerRegistrationExtensions
    {
        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingRedis configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        public static void AddProducer<T>(this IRiderRegistrationConfigurator configurator, string StreamName,
            Action<IRiderRegistrationContext, IRedisProducerConfigurator<Ignore, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(StreamName, _ => default, configure);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingRedis configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="producerConfig"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        public static void AddProducer<T>(this IRiderRegistrationConfigurator configurator, string StreamName,
            ProducerConfig producerConfig,
            Action<IRiderRegistrationContext, IRedisProducerConfigurator<Ignore, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(StreamName, producerConfig, _ => default, configure);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingRedis configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string StreamName,
            Action<IRiderRegistrationContext, IRedisProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(StreamName))
                throw new ArgumentException(nameof(StreamName));

            var registration = new RedisProducerRegistrationConfigurator<TKey, T>(StreamName, configure);
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(StreamName, provider));
            configurator.AddRegistration(registration);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingRedis configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="producerConfig"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string StreamName,
            ProducerConfig producerConfig,
            Action<IRiderRegistrationContext, IRedisProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(StreamName))
                throw new ArgumentException(nameof(StreamName));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            var registration = new RedisProducerRegistrationConfigurator<TKey, T>(StreamName, configure, producerConfig);
            configurator.Registrar.Register(provider => GetProducer<TKey, T>(StreamName, provider));
            configurator.AddRegistration(registration);
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingRedis configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="keyResolver">Key resolver</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string StreamName,
            RedisKeyResolver<TKey, T> keyResolver,
            Action<IRiderRegistrationContext, IRedisProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(StreamName, configure);
            configurator.Registrar.Register<IStreamProducer<T>>(provider =>
                new KeyedStreamProducer<TKey, T>(provider.GetRequiredService<IStreamProducer<TKey, T>>(), keyResolver));
        }

        /// <summary>
        /// Add a provider to the container for the specified message type, using a key type of Null
        /// The producer must be configured in the UsingRedis configuration method.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="producerConfig"></param>
        /// <param name="keyResolver">Key resolver</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        public static void AddProducer<TKey, T>(this IRiderRegistrationConfigurator configurator, string StreamName,
            ProducerConfig producerConfig, RedisKeyResolver<TKey, T> keyResolver,
            Action<IRiderRegistrationContext, IRedisProducerConfigurator<TKey, T>> configure = null)
            where T : class
        {
            configurator.AddProducer(StreamName, producerConfig, configure);
            configurator.Registrar.Register<IStreamProducer<T>>(provider =>
                new KeyedStreamProducer<TKey, T>(provider.GetRequiredService<IStreamProducer<TKey, T>>(), keyResolver));
        }

        static IStreamProducer<TKey, T> GetProducer<TKey, T>(string StreamName, IConfigurationServiceProvider provider)
            where T : class
        {
            var address = new Uri($"Stream:{StreamName}");

            return GetProducer<TKey, T>(address, provider);
        }

        static IStreamProducer<TKey, T> GetProducer<TKey, T>(Uri address, IConfigurationServiceProvider provider)
            where T : class
        {
            var rider = provider.GetRequiredService<IRedisRider>();

            var contextProvider = provider.GetService<ScopedConsumeContextProvider>();
            if (contextProvider != null)
            {
                return contextProvider.HasContext
                    ? rider.GetProducer<TKey, T>(address, contextProvider.GetContext())
                    : rider.GetProducer<TKey, T>(address);
            }

            return rider.GetProducer<TKey, T>(address, provider.GetService<ConsumeContext>());
        }
    }
}
