namespace MassTransit.RedisIntegration
{
    using System;
    using System.Diagnostics;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct RedisStreamAddress
    {
        public const string PathPrefix = "Redis";

        public readonly string Stream;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public RedisStreamAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            Stream = default;
            Scheme = default;
            Port = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "Stream":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port);
                    Stream = address.AbsolutePath;
                    break;
                default:
                {
                    if (string.Equals(address.Scheme, hostAddress.Scheme, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseLeft(hostAddress, out Scheme, out Host, out Port);
                        Stream = address.AbsolutePath.Replace($"{PathPrefix}/", "");
                    }
                    else
                        throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));

                    break;
                }
            }
        }

        public RedisStreamAddress(Uri hostAddress, string Stream)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);

            Stream = Stream;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port)
        {
            scheme = address.Scheme;
            host = address.Host;
            port = address.Port;
        }

        public static implicit operator Uri(in RedisStreamAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port ?? 0,
                Path = $"{PathPrefix}/{address.Stream}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
