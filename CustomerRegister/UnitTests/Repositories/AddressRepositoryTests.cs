using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures.Structs;
using Services.Repositories;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Repositories
{
    public class AddressRepositoryTests : DatabaseTestsBase
    {
        private readonly IServiceProvider _serviceProvider;
        
        const string City = "Palmas";
        const string ZipCode = "77001-004";
        const string Country = "Brazil";
        const string State = "Tocantins";
        const string Street = "Non Existing Street";
        const int Number = 70;

        public AddressRepositoryTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddTestDatabase(DatabasePath)
                .AddLogging()
                .AddUnitOfWork()
                .AddRepositories()
                .BuildServiceProvider();
        }
        
        private static Address GetDummyAddress()
        {
            return new Address
            {
                City = City,
                Country = Country,
                State = State,
                ZipCode = ZipCode,
                Street = Street,
                Number = Number
            };
        }

        private static Address GetDummyAddress(Guid uuid)
        {
            var address = GetDummyAddress();
            address.Uuid = uuid;

            return address;
        }
        
        private static void AssertDummyAddress(Address address)
        {
            Assert.Equal(City, address.City);
            Assert.Equal(Country, address.Country);
            Assert.Equal(State, address.State);
            Assert.Equal(ZipCode, address.ZipCode);
            Assert.Equal(Street, address.Street);
            Assert.Equal(Number, address.Number);
        }

        [Fact]
        public async Task SaveShouldAddANewAddressGiverNonExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var address = GetDummyAddress();
            sut.Save(address);
            var savedChanges = await unitOfWork.SaveChangesAsync();
            var savedAddress = await context.Addresses.FirstAsync();

            Assert.True(savedChanges);
            AssertDummyAddress(savedAddress);
            Assert.NotEqual(default, savedAddress.Uuid);
        }

        [Fact]
        public async Task GetShouldReturnAnExistingAddress()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var uuid = Guid.NewGuid();
            var addressToRetrieve = GetDummyAddress(uuid);

            await context.Addresses.AddAsync(addressToRetrieve);
            await context.SaveChangesAsync();
            var retrievedAddress = await sut.GetAsync(uuid);
            
            Assert.NotNull(retrievedAddress);
            AssertDummyAddress(retrievedAddress);
            Assert.Equal(uuid, retrievedAddress.Uuid);
        }

        [Fact]
        public async Task GetShouldReturnNullGivenInactiveRecord()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var uuid = Guid.NewGuid();
            var addressToRetrieve = GetDummyAddress(uuid);
            addressToRetrieve.IsActive = false;

            await context.Addresses.AddAsync(addressToRetrieve);
            await context.SaveChangesAsync();
            var retrievedAddress = await sut.GetAsync(uuid);
            
            Assert.Null(retrievedAddress);
        }
        
        [Theory]
        [InlineData(3, 10)]
        [InlineData(15,10)]
        public async Task ListAsyncShouldReturnAPaginationWithEntries(int total, int perPage)
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await context.Addresses.AddAsync(GetDummyAddress(Guid.NewGuid()));
            await context.SaveChangesAsync();

            var pagination = new PaginationData
            {
                CurrentPage = 1,
                PerPage = perPage
            };
            var result = await sut.ListAsync(pagination);
            var addresses = result.Elements;

            var limit = total < perPage ? total : perPage ;
            Assert.Equal( limit, addresses.Count());
            for (var i = 0; i < limit; i++)
            {
                var currentAddress = addresses.ElementAt(i);
                AssertDummyAddress(currentAddress);
                Assert.NotEqual(default, currentAddress.Uuid);
            }
            Assert.Equal(total, result.Total);
        }

        [Fact]
        public async Task ListAsyncShouldReturn5EntriesGivenSecondPageOf15Entries()
        {
            const int total = 15;
            const int page = 2;
            const int perPage = 10;
            const int expectedSecondPage = 5;
            
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await context.Addresses.AddAsync(GetDummyAddress(Guid.NewGuid()));
            await context.SaveChangesAsync();

            var pagination = new PaginationData
            {
                CurrentPage = page,
                PerPage = perPage
            };
            var result = await sut.ListAsync(pagination);
            var addresses = result.Elements;
            
            Assert.Equal( expectedSecondPage, addresses.Count());
            for (var i = 0; i < expectedSecondPage; i++)
            {
                var currentAddress = addresses.ElementAt(i);
                AssertDummyAddress(currentAddress);
                Assert.NotEqual(default, currentAddress.Uuid);
            }
            Assert.Equal(total, result.Total);
        }

        [Fact]
        public async Task ListAsyncShouldSkipInactiveItems()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var address = GetDummyAddress(Guid.NewGuid());
            address.IsActive = false;
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();

            var pagination = new PaginationData
            {
                CurrentPage = 1,
                PerPage = 10
            };
            var result = await sut.ListAsync(pagination);
            var addresses = result.Elements;
            
            Assert.Empty(result.Elements);
            Assert.Equal(0, result.Total);
        }

        [Fact]
        public async Task EditShouldUpdateANewEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
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

            var editedAddress = new Address
            {
                City = newCity,
                Country = newCountry,
                Number = newNumber,
                State = newState,
                Street = newStreet,
                Uuid = uuid,
                ZipCode = newZipCode,
                Id = address.Id
            };

            sut.Save(editedAddress);
            await unitOfWork.SaveChangesAsync();

            var insertedAddress = await context.Addresses.FirstAsync();
            Assert.Equal(newCity, insertedAddress.City);
            Assert.Equal(newCountry, insertedAddress.Country);
            Assert.Equal(newState, insertedAddress.State);
            Assert.Equal(newZipCode, insertedAddress.ZipCode);
            Assert.Equal(newStreet, insertedAddress.Street);
            Assert.Equal(newNumber, insertedAddress.Number);
            Assert.Equal(uuid, insertedAddress.Uuid);
        }
        
    }
}