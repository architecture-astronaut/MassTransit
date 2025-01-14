namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Registration;
    using Riders;
    using Transports;
    using Util;


    public class RedisRider :
        IRedisRider
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointCollection _endpoints;
        readonly IRedisHostConfiguration _hostConfiguration;
        readonly IStreamProducerProvider _producerProvider;
        readonly IRiderRegistrationContext _registrationContext;

        public RedisRider(IRedisHostConfiguration hostConfiguration, IBusInstance busInstance, IReceiveEndpointCollection endpoints,
            IStreamProducerProvider producerProvider, IRiderRegistrationContext registrationContext)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpoints = endpoints;
            _producerProvider = producerProvider;
            _registrationContext = registrationContext;
        }

        public IStreamProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext)
            where TValue : class
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var provider = consumeContext == null
                ? _producerProvider
                : new ConsumeContextStreamProducerProvider(_producerProvider, consumeContext);

            return provider.GetProducer<TKey, TValue>(address);
        }

        public HostReceiveEndpointHandle ConnectStreamEndpoint<TKey, TValue>(string StreamName, string groupId,
            Action<IRiderRegistrationContext, IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = _hostConfiguration.CreateSpecification<TKey, TValue>(StreamName, groupId, configurator =>
            {
                configure?.Invoke(_registrationContext, configurator);
            });

            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));

            return _endpoints.Start(specification.EndpointName);
        }

        public HostReceiveEndpointHandle ConnectStreamEndpoint<TKey, TValue>(string StreamName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IRedisStreamReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = _hostConfiguration.CreateSpecification<TKey, TValue>(StreamName, consumerConfig, configurator =>
            {
                configure?.Invoke(_registrationContext, configurator);
            });

            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));

            return _endpoints.Start(specification.EndpointName);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);

            var ready = endpointsHandle.Length == 0 ? TaskUtil.Completed : _hostConfiguration.ClientContextSupervisor.Ready;

            var agent = new RiderAgent(_hostConfiguration.ClientContextSupervisor, _endpoints, ready);

            return new Handle(endpointsHandle, agent);
        }


        class RiderAgent :
            Agent
        {
            readonly IReceiveEndpointCollection _endpoints;
            readonly IClientContextSupervisor _supervisor;

            public RiderAgent(IClientContextSupervisor supervisor, IReceiveEndpointCollection endpoints, Task ready)
            {
                _supervisor = supervisor;
                _endpoints = endpoints;

                SetReady(ready);
                SetCompleted(_supervisor.Completed);
            }

            protected override async Task StopAgent(StopContext context)
            {
                await _endpoints.Stop(context.CancellationToken).ConfigureAwait(false);
                await _supervisor.Stop(context).ConfigureAwait(false);
                await base.StopAgent(context).ConfigureAwait(false);
            }
        }


        class Handle :
            RiderHandle
        {
            readonly IAgent _agent;
            readonly HostReceiveEndpointHandle[] _endpoints;

            public Handle(HostReceiveEndpointHandle[] endpoints, IAgent agent)
            {
                _endpoints = endpoints;
                _agent = agent;
            }

            public Task Ready => ReadyOrNot(_endpoints.Select(x => x.Ready));

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return _agent.Stop("Redis stopped", cancellationToken);
            }

            async Task ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
            {
                Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
                foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                    await ready.ConfigureAwait(false);

                await _agent.Ready.ConfigureAwait(false);

                await Task.WhenAll(readyTasks).ConfigureAwait(false);
            }
        }
    }
}
