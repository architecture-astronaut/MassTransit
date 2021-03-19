namespace MassTransit.RedisIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Redis messages are a combination of headers, a key, and a value.
    /// </summary>
    /// <typeparam name="TKey">The Redis Stream key type</typeparam>
    /// <typeparam name="TValue">The Redis Stream value type</typeparam>
    public interface IStreamProducer<TKey, TValue> :
        ISendObserverConnector
        where TValue : class
    {
        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="key">The key, matching the Stream type</param>
        /// <param name="value">The message value</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="key">The key, matching the Stream type</param>
        /// <param name="value">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, TValue value, IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="key">The key, matching the Stream type</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, object values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="key">The key, matching the Stream type</param>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TKey key, object values, IPipe<RedisSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// Redis messages are a combination of headers, a Null key type, and a value.
    /// </summary>
    /// <typeparam name="TValue">The Redis Stream value type</typeparam>
    public interface IStreamProducer<TValue> :
        ISendObserverConnector
        where TValue : class
    {
        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="message">The message value</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TValue message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="message">The message value</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(TValue message, IPipe<RedisSendContext<TValue>> pipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="cancellationToken"></param>
        Task Produce(object values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Produces a message to the configured Redis Stream.
        /// </summary>
        /// <param name="values">An object which is used to initialize the message</param>
        /// <param name="pipe">A pipe which is called to customize the produced message context</param>
        /// <param name="cancellationToken"></param>
        Task Produce(object values, IPipe<RedisSendContext<TValue>> pipe, CancellationToken cancellationToken = default);
    }
}
