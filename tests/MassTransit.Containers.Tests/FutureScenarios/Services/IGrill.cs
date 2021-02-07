namespace MassTransit.Containers.Tests.FutureScenarios.Services
{
    using System.Threading.Tasks;
    using Contracts;


    public interface IGrill
    {
        Task<BurgerPatty> CookOrUseExistingPatty(decimal weight, bool cheese);
        void Add(BurgerPatty patty);
    }
}
