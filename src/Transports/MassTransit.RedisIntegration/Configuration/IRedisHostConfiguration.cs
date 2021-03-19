namespace MassTransit.RedisIntegration
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Specifications;
    using Transport;


    public interface IRedisHostConfiguration :
        ISpecification
    {
        IReadOnlyDictionary<string, string> Configuration { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }

        IRedisConsumerSpecification CreateSpecification<TKey, TValue>(string StreamName, string groupId,
            Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IRedisConsumerSpecification CreateSpecification<TKey, TValue>(string StreamName, ConsumerConfig consumerConfig,
            Action<IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class;

        IRedisRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
