using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Database;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Models.Address;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class AddressControllerTests : IClassFixture<ApplicationFactory>, IDisposable
    {
        private readonly ApplicationFactory _factory;
        private readonly IServiceProvider _serviceProvider;

        public AddressControllerTests(ApplicationFactory factory)
        {
            _factory = factory;
            _serviceProvider = new ServiceCollection()
                .AddTestDatabase(_factory.DatabasePath)
                .BuildServiceProvider();
        }

        [Fact]
        public async Task AddShouldAddANewAddress()
        {
            var client = _factory.CreateClient();
            var address = new SaveAddressModel
            {
                City = "Test City",
                Country = "Test Country",
                Number = 100,
                State = "Test State",
                ZipCode = "00000-000",
                Street = "Test Street"
            };

            var serializedJson = JsonSerializer.Serialize(address);
            var contentJson = new StringContent(serializedJson, Encoding.UTF8, "application/json");

            var result = await client.PostAsync("Address/Add", contentJson);
            result.EnsureSuccessStatusCode();

            using var scope = _serviceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            Assert.NotEmpty(context.Addresses);
            Assert.Equal(1,context.Addresses.Count());

        }

        public void Dispose()
        {
            _factory?.Dispose();
        }
    }
}