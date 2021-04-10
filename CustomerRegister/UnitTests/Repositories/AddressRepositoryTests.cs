using System;
using System.Linq;
using System.Threading.Tasks;
using CommonFixtures;
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
        private static void AssertDummyAddress(Address address)
        {
            Assert.Equal(AddressFixture.City, address.City);
            Assert.Equal(AddressFixture.Country, address.Country);
            Assert.Equal(AddressFixture.State, address.State);
            Assert.Equal(AddressFixture.ZipCode, address.ZipCode);
            Assert.Equal(AddressFixture.Street, address.Street);
            Assert.Equal(AddressFixture.Number, address.Number);
        }

        [Fact]
        public async Task SaveShouldAddANewAddressGiverNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var address = AddressFixture.GetDummyAddress();
            await sut.SaveAsync(address);
            var savedChanges = await unitOfWork.SaveChangesAsync();
            var savedAddress = await context.Addresses.FirstAsync();

            Assert.True(savedChanges);
            AssertDummyAddress(savedAddress);
            Assert.NotEqual(default, savedAddress.Uuid);
        }

        [Fact]
        public async Task GetShouldReturnAnExistingAddress()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressToRetrieve = await SeedDatabaseFixture.AddDummyAddressAsync(context);
            var uuid = addressToRetrieve.Uuid;
            var retrievedAddress = await sut.GetAsync(uuid);
            
            Assert.NotNull(retrievedAddress);
            AssertDummyAddress(retrievedAddress);
            Assert.Equal(uuid, retrievedAddress.Uuid);
        }

        [Fact]
        public async Task GetShouldReturnNullGivenInactiveRecord()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var addressToRetrieve = AddressFixture.GetDummyAddress(uuid);
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
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await SeedDatabaseFixture.AddDummyAddressAsync(context);

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
            
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await SeedDatabaseFixture.AddDummyAddressAsync(context);

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
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(uuid);
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
        public async Task SaveShouldUpdateANewEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var address = await SeedDatabaseFixture.AddDummyAddressAsync(context);

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
                Uuid = address.Uuid,
                ZipCode = newZipCode,
                Id = address.Id
            };

            await sut.SaveAsync(editedAddress);
            await unitOfWork.SaveChangesAsync();

            var insertedAddress = await context.Addresses.FirstAsync();
            Assert.Equal(newCity, insertedAddress.City);
            Assert.Equal(newCountry, insertedAddress.Country);
            Assert.Equal(newState, insertedAddress.State);
            Assert.Equal(newZipCode, insertedAddress.ZipCode);
            Assert.Equal(newStreet, insertedAddress.Street);
            Assert.Equal(newNumber, insertedAddress.Number);
            Assert.Equal(address.Uuid, insertedAddress.Uuid);
        }

        [Fact]
        public async Task DeleteShouldMarkIsActiveAsFalse()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var address = await SeedDatabaseFixture.AddDummyAddressAsync(context);

            await sut.DeleteAsync(address.Uuid);
            await unitOfWork.SaveChangesAsync();

            var deletedAddress = await context.Addresses.FirstAsync(x => x.Uuid == address.Uuid);
            Assert.False(deletedAddress.IsActive);
        }
        
        [Fact]
        public async Task DeleteAsyncShouldNotThrowExceptionsGivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await sut.DeleteAsync(Guid.NewGuid());
            await unitOfWork.SaveChangesAsync();
            Assert.True(true);
        }
    }
}