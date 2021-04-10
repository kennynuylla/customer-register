using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Models.Phone;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class PhoneControllerTests : ControllerTestsBase
    {
        public PhoneControllerTests(ApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AddShouldAddNewPhone()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            var request = new AddPhoneModel
            {
                Number = "7894-5698",
                AreaCode = "78",
                CustomerUuid = customer.Uuid
            };
            var serializedRequest = JsonSerializer.Serialize(request);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PostAsync("Phone/Add", contentRequest);
            result.EnsureSuccessStatusCode();
            var savedPhone = await context.Phones.Include(x => x.Customer).FirstAsync();
            
            Assert.Equal(request.Number, savedPhone.Number);
            Assert.Equal(request.AreaCode, savedPhone.AreaCode);
            Assert.Equal(request.CustomerUuid, savedPhone.Customer.Uuid);
        }
    }
}