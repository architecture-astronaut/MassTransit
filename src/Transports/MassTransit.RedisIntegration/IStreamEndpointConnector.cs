namespace MassTransit.RedisIntegration
{
    using System;
    using MassTransit.Registration;


    public interface IStreamEndpointConnector
    {
        HostReceiveEndpointHandle ConnectStreamEndpoint<TKey, TValue>(string StreamName, string groupId,
            Action<IRiderRegistrationContext, IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        HostReceiveEndpointHandle ConnectStreamEndpoint<TKey, TValue>(string StreamName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;
    }
}
