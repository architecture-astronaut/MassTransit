namespace MassTransit.RedisIntegration.Specifications
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public class RedisBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IRiderRegistrationContext _context;
        readonly IRedisHostConfiguration _hostConfiguration;

        public RedisBusInstanceSpecification(IRiderRegistrationContext context, IRedisHostConfiguration hostConfiguration)
        {
            _context = context;
            _hostConfiguration = hostConfiguration;
        }

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(_context, busInstance);
            busInstance.Connect<IRedisRider>(rider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _hostConfiguration.Validate();
        }
    }
}
