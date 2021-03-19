namespace MassTransit.RedisIntegration
{
    using System.Collections.Generic;
    using Configuration;
    using GreenPipes;


    public class RedisStreamOptions :
        IOptions,
        ISpecification
    {
        readonly string _Stream;
        ushort _numPartitions;
        short _replicaFactor;

        public RedisStreamOptions(string Stream)
        {
            _Stream = Stream;
            _numPartitions = 1;
            _replicaFactor = 1;
        }

        /// <summary>
        /// The number of partitions for the new Stream
        /// </summary>
        public ushort NumPartitions
        {
            set => _numPartitions = value;
        }

        /// <summary>
        /// The replication factor for the new Stream (should not be bigger than number of brokers)
        /// </summary>
        public short ReplicationFactor
        {
            set => _replicaFactor = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_numPartitions == 0)
                yield return this.Failure(nameof(NumPartitions), "should be greater than 0");

            if (_replicaFactor <= 0)
                yield return this.Failure(nameof(ReplicationFactor), "should be greater than 0");
        }

        internal StreamSpecification ToSpecification()
        {
            return new StreamSpecification
            {
                Name = _Stream,
                NumPartitions = _numPartitions,
                ReplicationFactor = _replicaFactor,
            };
        }
    }
}
