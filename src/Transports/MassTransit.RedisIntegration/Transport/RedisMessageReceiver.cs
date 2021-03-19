﻿namespace MassTransit.RedisIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Transports;
    using Util;


    public class RedisMessageReceiver<TKey, TValue> :
        Agent,
        IRedisMessageReceiver<TKey, TValue>
        where TValue : class
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly ConsumerContext<TKey, TValue> _consumerContext;
        readonly ReceiveEndpointContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IReceivePipeDispatcher _dispatcher;

        public RedisMessageReceiver(ReceiveEndpointContext context, ConsumerContext<TKey, TValue> consumerContext)
        {
            _context = context;
            _consumerContext = consumerContext;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Stopping);

            _consumerContext.ErrorHandler += HandleRedisError;

            _deliveryComplete = TaskUtil.GetTask<bool>();

            _dispatcher = context.CreateReceivePipeDispatcher();
            _dispatcher.ZeroActivity += HandleDeliveryComplete;

            Task.Run(Consume);
        }

        public long DeliveryCount => _dispatcher.DispatchCount;

        public int ConcurrentDeliveryCount => _dispatcher.MaxConcurrentDispatchCount;

        async Task Consume()
        {
            var prefetchCount = Math.Max(1000, _consumerContext.ReceiveSettings.CheckpointMessageCount / 10);
            var executor = new ChannelExecutor(prefetchCount, _consumerContext.ReceiveSettings.ConcurrencyLimit);

            await _consumerContext.Subscribe().ConfigureAwait(false);

            SetReady();

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    ConsumeResult<TKey, TValue> consumeResult = await _consumerContext.Consume(_cancellationTokenSource.Token).ConfigureAwait(false);
                    await executor.Push(() => Handle(consumeResult), Stopping).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == Stopping
                || exception.CancellationToken == _cancellationTokenSource.Token)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Consume Loop faulted");
            }
            finally
            {
                await executor.DisposeAsync().ConfigureAwait(false);
            }

            SetCompleted(TaskUtil.Completed);
        }

        async Task Handle(ConsumeResult<TKey, TValue> result)
        {
            if (IsStopping)
                return;

            var context = new ConsumeResultReceiveContext<TKey, TValue>(result, _context, _consumerContext, _consumerContext.HeadersDeserializer);

            try
            {
                await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
            }
            finally
            {
                context.Dispose();
            }
        }

        void HandleRedisError(IConsumer<TKey, TValue> consumer, Error error)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
                return;
            var activeDispatchCount = _dispatcher.ActiveDispatchCount;
            EnabledLogger? logger = error.IsFatal ? LogContext.Critical : LogContext.Error;
            logger?.Log("Consumer error ({Code}): {Reason} on {Stream}", error.Code, error.Reason, _consumerContext.ReceiveSettings.Stream);
            if (activeDispatchCount == 0 || error.IsLocalError)
            {
                _cancellationTokenSource.Cancel();
                _deliveryComplete.TrySetResult(true);
                SetCompleted(TaskUtil.Faulted<bool>(new RedisException(error)));
            }
        }

        async Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                LogContext.Debug?.Log("Consumer shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

        protected override async Task StopAgent(StopContext context)
        {
            await _consumerContext.Close().ConfigureAwait(false);

            _consumerContext.ErrorHandler -= HandleRedisError;

            LogContext.Debug?.Log("Stopping consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            if (_dispatcher.ActiveDispatchCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
            }
        }
    }
}
