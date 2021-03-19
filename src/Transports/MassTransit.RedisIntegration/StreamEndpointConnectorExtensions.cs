namespace MassTransit.RedisIntegration
{
    using System;
    using MassTransit.Registration;


    public static class StreamEndpointConnectorExtensions
    {
        /// <summary>
        /// Connect Stream endpoint
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="groupId">
        /// Client group id string. All clients sharing the same group.id belong to the same group.
        /// </param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static HostReceiveEndpointHandle ConnectStreamEndpoint<T>(this IStreamEndpointConnector connector, string StreamName, string groupId,
            Action<IRiderRegistrationContext, IRedisStreamReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            return connector.ConnectStreamEndpoint(StreamName, groupId, configure);
        }

        /// <summary>
        /// Connect Stream endpoint
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="StreamName">The Stream name</param>
        /// <param name="consumerConfig">Consumer config</param>
        /// <param name="configure"></param>
        /// <typeparam name="T">Message value type</typeparam>
        public static HostReceiveEndpointHandle ConnectStreamEndpoint<T>(this IStreamEndpointConnector connector, string StreamName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IRedisStreamReceiveEndpointConfigurator<Ignore, T>> configure)
            where T : class
        {
            return connector.ConnectStreamEndpoint(StreamName, consumerConfig, configure);
        }
    }
}
