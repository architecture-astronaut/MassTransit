namespace MassTransit.RedisIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public interface IRedisProducerSpecification :
        ISpecification
    {
        string StreamName { get; }
        IRedisProducerFactory CreateProducerFactory(IBusInstance busInstance);
    }
}
