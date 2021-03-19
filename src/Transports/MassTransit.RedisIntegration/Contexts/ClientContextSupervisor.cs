namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Registration;
    using Specifications;
    using Transport;
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        readonly ConcurrentDictionary<Uri, IRedisProducerFactory> _factories;
        readonly IReadOnlyDictionary<string, IRedisProducerSpecification> _producers;

        public ClientContextSupervisor(ClientConfig clientConfig, IEnumerable<IRedisProducerSpecification> producers)
            : base(new ClientContextFactory(clientConfig))
        {
            _producers = producers.ToDictionary(x => x.StreamName);
            _factories = new ConcurrentDictionary<Uri, IRedisProducerFactory>();
        }

        public IStreamProducer<TKey, TValue> CreateProducer<TKey, TValue>(IBusInstance busInstance, Uri address)
            where TValue : class
        {
            if (busInstance == null)
                throw new ArgumentNullException(nameof(busInstance));

            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var StreamAddress = NormalizeAddress(busInstance.HostConfiguration.HostAddress, address);

            IRedisProducerFactory<TKey, TValue> factory = GetFactory<TKey, TValue>(StreamAddress, busInstance);
            return factory.CreateProducer();
        }

        static RedisStreamAddress NormalizeAddress(Uri hostAddress, Uri address)
        {
            return new RedisStreamAddress(hostAddress, address);
        }

        IRedisProducerFactory<TKey, TValue> GetFactory<TKey, TValue>(RedisStreamAddress address, IBusInstance busInstance)
            where TValue : class
        {
            if (!_producers.TryGetValue(address.Stream, out var specification))
                throw new ConfigurationException($"Producer for Stream: {address} is not configured.");

            var factory = _factories.GetOrAdd(address, _ => specification.CreateProducerFactory(busInstance));

            if (factory is IRedisProducerFactory<TKey, TValue> f)
                return f;

            throw new ConfigurationException($"Producer for Stream: {address} is not configured for ${typeof(Message<TKey, TValue>).Name} message");
        }
    }
}
