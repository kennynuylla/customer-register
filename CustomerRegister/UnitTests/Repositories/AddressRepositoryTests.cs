using System;
using System.Threading.Tasks;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Repositories;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Repositories
{
    public class AddressRepositoryTests : DatabaseTestsBase
    {
        private readonly IServiceProvider _serviceProvider;

        public AddressRepositoryTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddTestDatabase(DatabasePath)
                .AddLogging()
                .AddUnitOfWork()
                .AddRepositories()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task SaveShouldAddANewAddressGiverNonExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            const string city = "Palmas";
            const string zipCode = "77001-004";
            const string country = "Brazil";
            const string state = "Tocantins";
            const string street = "Non Existing Street";
            const int number = 70;

            var address = new Address
            {
                City = city,
                Country = country,
                State = state,
                ZipCode = zipCode,
                Street = street,
                Number = number
            };
            sut.Save(address);
            var savedChanges = await unitOfWork.SaveChangesAsync();
            var savedAddress = await context.Addresses.FirstAsync();

            Assert.True(savedChanges);
            Assert.Equal(city, savedAddress.City);
            Assert.Equal(country, savedAddress.Country);
            Assert.Equal(state, savedAddress.State);
            Assert.Equal(zipCode, savedAddress.ZipCode);
            Assert.Equal(street, savedAddress.Street);
            Assert.Equal(number, savedAddress.Number);
            Assert.NotEqual(default, savedAddress.Uuid);
        }

        [Fact]
        public async Task GetShouldReturnEnExistingAddress()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            const string city = "Get Test";
            const string state = "Tocantins";
            const string country = "Brazil";
            const string zipCode = "12345-678";
            const string street = "Test";
            const int number = 8;
            var uuid = Guid.NewGuid();
            var addressToRetrieve = new Address
            {
                City = city,
                Country = country,
                Number = number,
                State = state,
                Street = street,
                ZipCode = zipCode,
                Uuid = uuid
            };

            await context.Addresses.AddAsync(addressToRetrieve);
            await context.SaveChangesAsync();
            var retrievedAddress = await sut.GetAsync(uuid);
            
            Assert.NotNull(retrievedAddress);
            Assert.Equal(city, retrievedAddress.City);
            Assert.Equal(country, retrievedAddress.Country);
            Assert.Equal(state, retrievedAddress.State);
            Assert.Equal(zipCode, retrievedAddress.ZipCode);
            Assert.Equal(street, retrievedAddress.Street);
            Assert.Equal(number, retrievedAddress.Number);
            Assert.Equal(uuid, retrievedAddress.Uuid);
        }
    }
}