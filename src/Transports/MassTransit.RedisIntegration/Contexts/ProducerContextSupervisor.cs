namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using Configuration;
    using Context;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using Serializers;
    using Transport;
    using Transports;


    public class ProducerContextSupervisor<TKey, TValue> :
        TransportPipeContextSupervisor<ProducerContext<TKey, TValue>>,
        IProducerContextSupervisor<TKey, TValue>
        where TValue : class
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservers;
        readonly ISendPipe _sendPipe;
        readonly RedisStreamAddress _StreamAddress;

        public ProducerContextSupervisor(string StreamName,
            ISendPipe sendPipe, SendObservable sendObservers, IClientContextSupervisor clientContextSupervisor,
            IHostConfiguration hostConfiguration, IHeadersSerializer headersSerializer,
            Func<ProducerBuilder<TKey, TValue>> producerBuilderFactory)
            : base(new ProducerContextFactory<TKey, TValue>(clientContextSupervisor, headersSerializer, producerBuilderFactory))
        {
            _sendObservers = sendObservers;
            _hostConfiguration = hostConfiguration;
            _StreamAddress = new RedisStreamAddress(hostConfiguration.HostAddress, StreamName);
            _sendPipe = sendPipe;

            clientContextSupervisor.AddSendAgent(this);
        }

        public IStreamProducer<TKey, TValue> CreateProducer()
        {
            var context = new RedisTransportContext(_sendPipe, _hostConfiguration, _StreamAddress);

            if (_sendObservers.Count > 0)
                context.ConnectSendObserver(_sendObservers);

            return new StreamProducer<TKey, TValue>(context, this);
        }


        class RedisTransportContext :
            BaseSendTransportContext,
            RedisSendTransportContext
        {
            public RedisTransportContext(ISendPipe sendPipe, IHostConfiguration hostConfiguration, RedisStreamAddress StreamAddress)
                : base(hostConfiguration)
            {
                SendPipe = sendPipe;
                HostAddress = hostConfiguration.HostAddress;
                StreamAddress = StreamAddress;
            }

            public Uri HostAddress { get; }
            public RedisStreamAddress StreamAddress { get; }
            public ISendPipe SendPipe { get; }
        }
    }
}
