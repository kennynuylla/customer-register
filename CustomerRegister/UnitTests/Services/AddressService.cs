using System;
using System.Threading.Tasks;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Services.Interfaces;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Services
{
    public class AddressService : DatabaseTestsBase
    {
        private readonly IServiceProvider _serviceProvider;

        public AddressService()
        {
            _serviceProvider = new ServiceCollection()
                .AddTestDatabase(DatabasePath)
                .AddLogging()
                .AddUnitOfWork()
                .AddRepositories()
                .AddServices()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task SaveShouldAddANewAddressGivenNonExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var address = new Address
            {
                City = "Porto Nacional",
                Country = "Brazil",
                Number = 300,
                State = "Tocantins",
                Street = "Test",
                ZipCode = "77500-000"
            };

            var result = sut.Save(address);
            await unitOfWork.SaveChangesAsync();

            Assert.True(result.IsSuccessful);
            Assert.NotEmpty(context.Addresses);
        }
    }
}