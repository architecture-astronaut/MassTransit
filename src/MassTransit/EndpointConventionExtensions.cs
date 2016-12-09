// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public static class EndpointConventionExtensions
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ISendEndpointProvider provider, T message, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ISendEndpointProvider provider, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ISendEndpointProvider provider, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ISendEndpointProvider provider, object message, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache.GetShortName(messageType)} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ISendEndpointProvider provider, object message, Type messageType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache.GetShortName(messageType)} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="messageType"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ISendEndpointProvider provider, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache.GetShortName(messageType)} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, messageType, pipe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="message">The message</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send(this ISendEndpointProvider provider, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache.GetShortName(messageType)} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="provider"></param>
        /// <param name="values"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ISendEndpointProvider provider, object values, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="provider"></param>
        /// <param name="values"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ISendEndpointProvider provider, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(values, pipe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="provider"></param>
        /// <param name="values"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        public static async Task Send<T>(this ISendEndpointProvider provider, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send<T>(values, pipe, cancellationToken).ConfigureAwait(false);
        }
    }
}