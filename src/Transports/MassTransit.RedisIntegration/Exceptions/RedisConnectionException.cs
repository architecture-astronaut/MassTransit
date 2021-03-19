namespace MassTransit.RedisIntegration.Exceptions
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class RedisConnectionException :
        ConnectionException
    {
        public RedisConnectionException()
        {
        }

        public RedisConnectionException(string message)
            : base(message)
        {
        }

        public RedisConnectionException(string message, Exception innerException)
            : base(message, innerException, IsExceptionTransient(innerException))
        {
        }

        protected RedisConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static bool IsExceptionTransient(Exception exception)
        {
            return exception switch
            {
                RedisException bue => bue.Error.IsLocalError,
                _ => true
            };
        }
    }
}
