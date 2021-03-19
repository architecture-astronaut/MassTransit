namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using MassTransit.Registration;
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
        IStreamProducer<TKey, TValue> CreateProducer<TKey, TValue>(IBusInstance busInstance, Uri address)
            where TValue : class;
    }
}
