namespace MassTransit.RedisIntegration
{
    using System;


    public interface ReceiveSettings
    {
        string Stream { get; }
        ushort CheckpointMessageCount { get; }
        int ConcurrencyLimit { get; }
        TimeSpan CheckpointInterval { get; }
    }
}
