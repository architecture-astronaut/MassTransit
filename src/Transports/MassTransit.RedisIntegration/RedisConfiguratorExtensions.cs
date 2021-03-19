namespace MassTransit
{
    using System;
    using RedisIntegration;


    public static class RedisConfiguratorExtensions
    {
        /// <summary>
        /// Configure Redis host
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="server"></param>
        /// <param name="configure"></param>
        public static void Host(this IRedisFactoryConfigurator configurator, string server, Action<IRedisHostConfigurator> configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            configurator.Host(new[] {server}, configure);
        }

        /// <summary>
        /// Subscribe to Redis Stream
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="groupId">
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static void StreamEndpoint<T>(this IRedisFactoryConfigurator configurator, string StreamName, string groupId,
            Action<IRedisStreamReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            configurator.StreamEndpoint(StreamName, groupId, configure);
        }

        /// <summary>
        /// Subscribe to Redis Stream
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="consumerConfig">Consumer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static void StreamEndpoint<T>(this IRedisFactoryConfigurator configurator, string StreamName, ConsumerConfig consumerConfig,
            Action<IRedisStreamReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            configurator.StreamEndpoint(StreamName, consumerConfig, configure);
        }

        /// <summary>
        /// Configure Redis Stream producer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Value key type</typeparam>
        internal static void StreamProducer<T>(this IRedisFactoryConfigurator configurator, string StreamName,
            Action<IRedisProducerConfigurator<Ignore, T>> configure)
            where T : class
        {
            configurator.StreamProducer(StreamName, configure);
        }

        /// <summary>
        /// Configure Redis Stream producer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="producerConfig">Producer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TValue">Value key type</typeparam>
        internal static void StreamProducer<TValue>(this IRedisFactoryConfigurator configurator, string StreamName, ProducerConfig producerConfig,
            Action<IRedisProducerConfigurator<Ignore, TValue>> configure)
            where TValue : class
        {
            configurator.StreamProducer(StreamName, producerConfig, configure);
        }

        /// <summary>
        /// Configure Redis Stream producer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="producerConfig">Producer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="TKey">Message key type</typeparam>
        /// <typeparam name="TValue">Value key type</typeparam>
        internal static void StreamProducer<TKey, TValue>(this IRedisFactoryConfigurator configurator, string StreamName, ProducerConfig producerConfig,
            Action<IRedisProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            configurator.StreamProducer(StreamName, producerConfig, configure);
        }
    }
}
