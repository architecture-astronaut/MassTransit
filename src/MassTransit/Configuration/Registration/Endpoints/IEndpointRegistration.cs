namespace MassTransit.Registration.Endpoints
{
    public interface IEndpointRegistration
    {
        IEndpointDefinition GetDefinition(IConfigurationServiceProvider provider);
    }
}
