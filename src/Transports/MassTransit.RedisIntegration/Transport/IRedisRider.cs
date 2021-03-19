namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using Riders;


    public interface IRedisRider :
        IRiderControl,
        IStreamEndpointConnector
    {
        IStreamProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext = default)
            where TValue : class;
    }
}
