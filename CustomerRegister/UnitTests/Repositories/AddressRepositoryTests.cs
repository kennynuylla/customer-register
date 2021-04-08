using System;
using System.Threading.Tasks;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    }
}