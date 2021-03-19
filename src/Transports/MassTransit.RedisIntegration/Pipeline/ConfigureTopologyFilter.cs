namespace MassTransit.RedisIntegration.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using Transport;


    public class ConfigureTopologyFilter<TKey, TValue> :
        IFilter<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly AdminClientConfig _config;
        readonly RedisStreamOptions _options;
        readonly StreamSpecification _specification;

        public ConfigureTopologyFilter(IReadOnlyDictionary<string, string> clientConfig, RedisStreamOptions options)
        {
            _options = options;
            _specification = _options.ToSpecification();
            _config = new AdminClientConfig(clientConfig.ToDictionary(x => x.Key, x => x.Value));
        }

        public async Task Send(ConsumerContext<TKey, TValue> context, IPipe<ConsumerContext<TKey, TValue>> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TKey, TValue>>(_ => CreateStream(), () => new Context()).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");
            scope.Add("specifications", _options);
        }

        async Task CreateStream()
        {
            var client = new AdminClientBuilder(_config).Build();
            try
            {
                var options = new CreateStreamsOptions {RequestTimeout = TimeSpan.FromSeconds(30)};
                LogContext.Debug?.Log("Creating Stream: {Stream}", _specification.Name);
                await client.CreateStreamsAsync(new[] {_specification}, options).ConfigureAwait(false);
            }
            catch (CreateStreamsException e)
            {
                EnabledLogger? logger = e.Error.IsFatal ? LogContext.Critical : LogContext.Error;
                logger?.Log("An error occured creating Streams. {Errors}", string.Join(", ", e.Results.Select(x => $"{x.Stream}:{x.Error.Reason}")));
            }
            finally
            {
                client.Dispose();
            }
        }


        class Context :
            ConfigureTopologyContext<TKey, TValue>
        {
        }
    }
}
