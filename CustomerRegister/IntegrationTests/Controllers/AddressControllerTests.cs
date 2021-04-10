using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures.Structs;
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
            var address = new AddAddressModel
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
        
        [Fact]
        public async Task GetShouldReturn404GivenNonExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = _factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var result = await sut.GetAsync($"Address/Get/{Guid.NewGuid()}");
            
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Theory]
        [InlineData(3, 10)]
        [InlineData(15, 8)]
        [InlineData(10, 10)]
        public async Task ListShouldReturnAListOfEntries(int total, int perPage)
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = _factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await context.Addresses.AddAsync(GetDummyAddress());
            await context.SaveChangesAsync();

            var result = await sut.GetAsync($"Address/List?currentPage=1&perPage={perPage}");
            result.EnsureSuccessStatusCode();
            var serializedResult = await result.Content.ReadAsStringAsync();
            var deserializedResult = JsonSerializer.Deserialize<PaginationResult<AddressListItem>>(serializedResult,
                scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());
            
            Assert.NotNull(deserializedResult);
            Assert.NotEmpty(deserializedResult.Elements);
            Assert.Equal(Math.Min(total, perPage), deserializedResult.Elements.Count());
            Assert.Equal(total, deserializedResult.Total);
            
        }

        [Fact]
        public async Task UpdateShouldUpdateAnExistingRecord()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = _factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var uuid = Guid.NewGuid();
            var address = GetDummyAddress(uuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            const string newCity = "Paraíso do Tocantins";
            const string newZipCode = "335045-879";
            const string newCountry = "Bengium";
            const string newState = "Test";
            const string newStreet = "Another street";
            const int newNumber = 800;

            var editedAddress = new UpdateAddressModel()
            {
                City = newCity,
                Country = newCountry,
                Number = newNumber,
                State = newState,
                Street = newStreet,
                ZipCode = newZipCode,
                Id = address.Id
            };
            
            var serializedJson = JsonSerializer.Serialize(editedAddress);
            var contentJson = new StringContent(serializedJson, Encoding.UTF8, "application/json");

            var result = await sut.PutAsync($"Address/Update/{uuid}", contentJson);
            var texto = await result.Content.ReadAsStringAsync();
            result.EnsureSuccessStatusCode();

            var uniqueAddress = await context.Addresses.FirstAsync();
            
            Assert.NotEmpty(context.Addresses);
            Assert.Equal(newCity, uniqueAddress.City);
            Assert.Equal(newCountry, uniqueAddress.Country);
            Assert.Equal(newState, uniqueAddress.State);
            Assert.Equal(newZipCode, uniqueAddress.ZipCode);
            Assert.Equal(newStreet, uniqueAddress.Street);
            Assert.Equal(newNumber, uniqueAddress.Number);
            Assert.Equal(uuid, uniqueAddress.Uuid);
            
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

        private Address GetDummyAddress(Guid uuid)
        {
            var address = GetDummyAddress();
            address.Uuid = uuid;

            return address;
        }

        public void Dispose()
        {
            _factory?.Dispose();
        }
    }
}