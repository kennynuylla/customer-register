using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Domain.Models;
using IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures.Structs;
using WebAPI.Models.Address;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class AddressControllerTests : ControllerTestsBase
    {
        
        public AddressControllerTests(ApplicationFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task AddShouldAddANewAddress()
        {
            var sut = Factory.CreateClient();
            var address = new AddAddressModel
            {
                City = AddressFixture.City,
                Country = AddressFixture.Country,
                Number = AddressFixture.Number,
                State = AddressFixture.State,
                ZipCode = AddressFixture.ZipCode,
                Street = AddressFixture.Street
            };

            var serializedJson = JsonSerializer.Serialize(address);
            var contentJson = new StringContent(serializedJson, Encoding.UTF8, "application/json");

            var result = await sut.PostAsync("Address/Add", contentJson);
            result.EnsureSuccessStatusCode();

            using var scope = ServiceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            Assert.NotEmpty(context.Addresses);
            Assert.Equal(1,context.Addresses.Count());
        }

        [Fact]
        public async Task GetShouldReturnExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress();
            address.Uuid = uuid;
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();

            var result = await sut.GetAsync($"Address/Get/{uuid}");
            result.EnsureSuccessStatusCode();

            var serializedResult = await result.Content.ReadAsStringAsync();
            var deserializedResult = JsonSerializer.Deserialize<Address>(serializedResult, scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());
            
            Assert.NotNull(deserializedResult);
            Assert.Equal(AddressFixture.City, deserializedResult.City);
            Assert.Equal(AddressFixture.Country, deserializedResult.Country);
            Assert.Equal(AddressFixture.Street, deserializedResult.Street);
            Assert.Equal(AddressFixture.ZipCode, deserializedResult.ZipCode);
            Assert.Equal(AddressFixture.State, deserializedResult.State);
            Assert.Equal(AddressFixture.Number, deserializedResult.Number);
        }
        
        [Fact]
        public async Task GetShouldReturn404GivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
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
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await context.Addresses.AddAsync(AddressFixture.GetDummyAddress());
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
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(uuid);
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

        [Fact]
        public async Task DeleteShouldSetIsActiveToFalse()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            await context.AddAsync(AddressFixture.GetDummyAddress(uuid));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await sut.DeleteAsync($"Address/Delete/{uuid}");
            result.EnsureSuccessStatusCode();

            var deletedAddress = await context.Addresses.FirstAsync(x => x.Uuid == uuid);
            Assert.False(deletedAddress.IsActive);
        }
    }
}