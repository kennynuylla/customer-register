using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Models.Customer;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class CustomerControllerTests : ControllerTestsBase
    {
        public CustomerControllerTests(ApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AddShouldCreateANewCustomer()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var customerToAdd = new AddCustomerModel
            {
                Email = "cicero@customer.com",
                Name = CustomerFixture.Name
            };
            var serializedRequest = JsonSerializer.Serialize(customerToAdd);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PostAsync("Customer/Add", contentRequest);
            result.EnsureSuccessStatusCode();

            var insertedCustomer = await context.Customers.FirstAsync();
            Assert.Equal(customerToAdd.Email, insertedCustomer.Email);
            Assert.Equal(customerToAdd.Name, insertedCustomer.Name);
            Assert.NotEqual(default, insertedCustomer.Uuid);
        }
    }
}