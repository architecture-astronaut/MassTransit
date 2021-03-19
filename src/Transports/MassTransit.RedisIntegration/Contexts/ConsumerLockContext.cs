namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Util;


    public class ConsumerLockContext<TKey, TValue> :
        IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        readonly SingleThreadedDictionary<Partition, PartitionCheckpointData> _data = new SingleThreadedDictionary<Partition, PartitionCheckpointData>();
        readonly IHostConfiguration _hostConfiguration;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;

        public ConsumerLockContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
        {
            _hostConfiguration = hostConfiguration;
            _timeout = receiveSettings.CheckpointInterval;
            _maxCount = receiveSettings.CheckpointMessageCount;
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.Partition, out var data))
                data.TryCheckpoint(result);

            return TaskUtil.Completed;
        }

        public void OnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<StreamPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (_data.TryAdd(partition.Partition, p => new PartitionCheckpointData(partition, consumer, _timeout, _maxCount)))
                    LogContext.Info?.Log("Partition: {PartitionId} was assigned", partition);
            }
        }

        public void OnUnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<StreamPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (_data.TryRemove(partition.Partition, out var data))
                    data.Close(partition);
            }
        }
    }
}
