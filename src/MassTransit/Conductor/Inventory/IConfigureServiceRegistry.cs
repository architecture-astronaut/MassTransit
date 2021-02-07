namespace MassTransit.Conductor.Inventory
{
    public interface IConfigureServiceRegistry
    {
        void Configure(IServiceRegistry registry);
    }
}
