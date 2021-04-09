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
        
        private const string City = "Porto Nacional";
        private const string State = "Tocantins";
        private const string Country = "Brazil";
        private const string ZipCode = "77500-000";
        private const string Street = "Test";
        private const int Number = 8;

        public AddressControllerTests(ApplicationFactory factory)
        {
            _factory = factory;
            _serviceProvider = new ServiceCollection()
                .AddTestDatabase(_factory.DatabasePath)
                .AddSingleton(sp => new JsonSerializerOptions {PropertyNameCaseInsensitive = true})
                .BuildServiceProvider();
        }

        [Fact]
        public async Task AddShouldAddANewAddress()
        {
            var sut = _factory.CreateClient();
            var address = new SaveAddressModel
            {
                City = City,
                Country = Country,
                Number = Number,
                State = State,
                ZipCode = ZipCode,
                Street = Street
            };

            var serializedJson = JsonSerializer.Serialize(address);
            var contentJson = new StringContent(serializedJson, Encoding.UTF8, "application/json");

            var result = await sut.PostAsync("Address/Add", contentJson);
            result.EnsureSuccessStatusCode();

            using var scope = _serviceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            Assert.NotEmpty(context.Addresses);
            Assert.Equal(1,context.Addresses.Count());

        }

        [Fact]
        public async Task GetShouldReturnExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = _factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = GetDummyAddress();
            address.Uuid = uuid;
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();

            var result = await sut.GetAsync($"Address/Get/{uuid}");
            result.EnsureSuccessStatusCode();

            var serializedResult = await result.Content.ReadAsStringAsync();
            var deserializedResult = JsonSerializer.Deserialize<Address>(serializedResult, scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());
            
            Assert.NotNull(deserializedResult);
            Assert.Equal(City, deserializedResult.City);
            Assert.Equal(Country, deserializedResult.Country);
            Assert.Equal(Street, deserializedResult.Street);
            Assert.Equal(ZipCode, deserializedResult.ZipCode);
            Assert.Equal(State, deserializedResult.State);
            Assert.Equal(Number, deserializedResult.Number);
        }

        private Address GetDummyAddress() => new Address
        {
            City = City,
            Country = Country,
            Number = Number,
            State = State,
            Street = Street,
            ZipCode = ZipCode
        };

        public void Dispose()
        {
            _factory?.Dispose();
        }
    }
}