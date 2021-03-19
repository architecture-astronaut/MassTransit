namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using Context;
    using MassTransit.Pipeline;


    public interface RedisSendTransportContext :
        SendTransportContext
    {
        Uri HostAddress { get; }
        RedisStreamAddress StreamAddress { get; }
        ISendPipe SendPipe { get; }
    }
}
