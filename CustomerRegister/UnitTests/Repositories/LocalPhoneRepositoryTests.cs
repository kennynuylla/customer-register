using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Microsoft.Extensions.DependencyInjection;
using Services.Repositories;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Repositories
{
    public class LocalPhoneRepositoryTests : DatabaseTestsBase
    {
        [Fact]
        public async Task GetGuidsFromAddressShouldReturnAListOfGuids()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var (address, phone) = await SeedDatabaseFixture.AddPhoneAndAddressAsync(context);

            var result = await sut.GetGuidsFromAddress(address.Uuid);
            
            Assert.NotEmpty(result);
            Assert.Contains(phone.Uuid, result);
        }
    }
}