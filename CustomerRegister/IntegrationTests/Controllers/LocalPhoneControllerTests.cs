using System;
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
using Services.Services.Interfaces;
using WebAPI.Models.LocalPhone;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class LocalPhoneControllerTests : ControllerTestsBase
    {
        public LocalPhoneControllerTests(ApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AddShouldAddANewEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            await context.Addresses.AddAsync(AddressFixture.GetDummyAddress(addressUuid));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var request = new AddLocalPhoneModel
            {
                Number = LocalPhoneFixture.Number,
                AreaCode = LocalPhoneFixture.AreaCode,
                AddressUuid = addressUuid
            };

            var serializedRequest = JsonSerializer.Serialize(request);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PostAsync("LocalPhone/Add", contentRequest);
            result.EnsureSuccessStatusCode();

            var insertedPhone = await context.LocalPhones.Include(x => x.PhoneAddress).FirstAsync();
            
            Assert.Equal(LocalPhoneFixture.Number, insertedPhone.Number);
            Assert.Equal(LocalPhoneFixture.AreaCode, insertedPhone.AreaCode);
            Assert.NotNull(insertedPhone.PhoneAddress);
        }

        [Fact]
        public async Task GetShouldReturn404GivenNonExistingEntry()
        {
            var sut = Factory.CreateClient();
            var result = await sut.GetAsync($"LocalPhone/Get/{Guid.NewGuid()}");
            
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetShouldReturnDetailedDataFromTheEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(addressUuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var phoneUuid = Guid.NewGuid();
            var phone = LocalPhoneFixture.GetDummyLocalPhone(phoneUuid, address.Id);
            await context.LocalPhones.AddAsync(phone);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await sut.GetAsync($"LocalPhone/Get/{phoneUuid}");
            result.EnsureSuccessStatusCode();
            var serializedResult = await result.Content.ReadAsStringAsync();
            var detailedPhone =  JsonSerializer.Deserialize<LocalPhone>(serializedResult, ServiceProvider.GetRequiredService<JsonSerializerOptions>());

            var detailedAddress = detailedPhone.PhoneAddress;
            
            Assert.Equal(LocalPhoneFixture.Number, detailedPhone.Number);
            Assert.Equal(LocalPhoneFixture.AreaCode, detailedPhone.AreaCode);
            Assert.Equal(AddressFixture.Country, detailedAddress.Country);
            Assert.Equal(AddressFixture.ZipCode, detailedAddress.ZipCode);
            Assert.Equal(AddressFixture.City, detailedAddress.City);
            Assert.Equal(AddressFixture.Number, detailedAddress.Number);
            Assert.Equal(AddressFixture.State, detailedAddress.State);
            Assert.Equal(AddressFixture.Street, detailedAddress.Street);
        }

        [Fact]
        public async Task ListShouldNotThrowExceptionsGivenEmptyDatabase()
        {
            var sut = Factory.CreateClient();
            var result = await sut.GetAsync("LocalPhone/List?currentPage=1&perPage=10");
            result.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ListShouldReturnAListOfEntries()
        {
            const int total = 5;
            const int perPage = 8;
            
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(addressUuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            for (var i = 0; i < total; i++) await context.LocalPhones.AddAsync(LocalPhoneFixture.GetDummyLocalPhone(Guid.NewGuid(), address.Id));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await sut.GetAsync($"LocalPhone/List?currentPage=1&perPage={perPage}");
            result.EnsureSuccessStatusCode();
            var serializedResult = await result.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<PaginationResult<LocalPhoneListItemModel>>(serializedResult,
                scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());

            foreach (var localPhone in list.Elements)
            {
                Assert.Equal(LocalPhoneFixture.Number, localPhone.Number);
                Assert.Equal(LocalPhoneFixture.AreaCode, localPhone.AreaCode);
                Assert.Equal(addressUuid, localPhone.AddressUuid);
                Assert.NotEqual(default, localPhone.AddressDescription);
            }
        }

        [Fact]
        public async Task UpdateShouldUpdateExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(addressUuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var phoneUuid = Guid.NewGuid();
            var phone = LocalPhoneFixture.GetDummyLocalPhone(phoneUuid, address.Id);
            await context.LocalPhones.AddAsync(phone);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            const string newNumber = "54879";
            var newPhone = new UpdateLocalPhoneModel
            {
                Id = phone.Id,
                Number = newNumber,
                AreaCode = phone.AreaCode,
                AddressUuid = addressUuid
            };
            var serializedRequest = JsonSerializer.Serialize(newPhone);
            var contentRequest = new StringContent(serializedRequest, Encoding.Default, "application/json");
            var result = await sut.PutAsync($"LocalPhone/Update/{phoneUuid}", contentRequest);
            result.EnsureSuccessStatusCode();

            var insertedPhone = await context.LocalPhones.FirstAsync();

            Assert.Equal(phone.Id, insertedPhone.Id);
            Assert.Equal(phone.Uuid, insertedPhone.Uuid);
            Assert.Equal(newNumber, insertedPhone.Number);
            Assert.Equal(phone.AreaCode, insertedPhone.AreaCode);
            Assert.Equal(phone.PhoneAddressId, insertedPhone.PhoneAddressId);
        }
    }
}