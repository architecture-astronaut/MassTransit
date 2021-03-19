namespace MassTransit.RedisIntegration
{
    public interface IRedisSslConfigurator
    {
        bool UseSsl { set; }
        string SslHost { set; }
        System.Security.Authentication.SslProtocols SslProtocols { set; }
    }
}
