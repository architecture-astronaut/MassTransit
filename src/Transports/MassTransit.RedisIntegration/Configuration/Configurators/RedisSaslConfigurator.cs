namespace MassTransit.RedisIntegration.Configurators
{
    public class RedisSaslConfigurator :
        IRedisSaslConfigurator
    {
        readonly ClientConfig _clientConfig;

        public RedisSaslConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public SaslMechanism? Mechanism
        {
            set => _clientConfig.SaslMechanism = value;
        }

        public string KerberosServiceName
        {
            set => _clientConfig.SaslKerberosServiceName = value;
        }

        public string KerberosPrincipal
        {
            set => _clientConfig.SaslKerberosPrincipal = value;
        }

        public string KerberosKinitCmd
        {
            set => _clientConfig.SaslKerberosKinitCmd = value;
        }

        public string KerberosKeytab
        {
            set => _clientConfig.SaslKerberosKeytab = value;
        }

        public int? KerberosMinTimeBeforeRelogin
        {
            set => _clientConfig.SaslKerberosMinTimeBeforeRelogin = value;
        }

        public string Username
        {
            set => _clientConfig.SaslUsername = value;
        }

        public string Password
        {
            set => _clientConfig.SaslPassword = value;
        }

        public string OauthbearerConfig
        {
            set => _clientConfig.SaslOauthbearerConfig = value;
        }

        public bool? EnableOauthbearerUnsecureJwt
        {
            set => _clientConfig.EnableSaslOauthbearerUnsecureJwt = value;
        }
    }
}
