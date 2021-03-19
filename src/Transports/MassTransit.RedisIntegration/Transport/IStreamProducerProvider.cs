namespace MassTransit.RedisIntegration.Transport
{
    using System;


    public interface IStreamProducerProvider
    {
        IStreamProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class;
    }
}
