namespace MassTransit.Containers.Tests.FutureScenarios.Services
{
    using System.Threading.Tasks;
    using Contracts;


    public class Fryer :
        IFryer
    {
        public async Task CookOnionRings(int quantity)
        {
        }

        public async Task CookFry(Size size)
        {
        }
    }
}
