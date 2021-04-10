using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.DataStructures;
using Services.DataStructures.Structs;
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
        
        private static Address GetDummyAddress()
        {
            return new Address
            {
                City = "Porto Nacional",
                Country = "Brazil",
                Number = 300,
                State = "Tocantins",
                Street = "Test",
                ZipCode = "77500-000"
            };
        }

        private static Address GetDummyAddress(Guid uuid)
        {
            var address = GetDummyAddress();
            address.Uuid = uuid;

            return address;
        }

        [Fact]
        public async Task SaveShouldAddANewAddressGivenNonExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var address = GetDummyAddress();

            var result = sut.Save(address);
            await unitOfWork.SaveChangesAsync();

            Assert.True(result.IsSuccessful);
            Assert.NotEmpty(context.Addresses);
            Assert.Equal(1,context.Addresses.Count());

        }
        
        [Fact]
        public async Task SaveShouldEditExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = GetDummyAddress(uuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            const string edited = "Edited";
            var editedAddress = new Address
            {
                City = address.City,
                Country = edited,
                Id = address.Id,
                Number = address.Number,
                Street = address.Street,
                Uuid = uuid,
                ZipCode = address.ZipCode,
                State = address.State
            };

            var result = sut.Save(editedAddress);
            var uniqueAddress = await context.Addresses.FirstAsync();
            
            Assert.True(result.IsSuccessful);
            Assert.Equal(edited, uniqueAddress.Country);
            Assert.Equal(uuid, uniqueAddress.Uuid);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnFailResultGivenNonExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();

            var result = await sut.DetailAsync(Guid.NewGuid());
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnDataGivenExistingEntry()
        {
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = GetDummyAddress(uuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();

            var result = await sut.DetailAsync(uuid);
            var success = (SuccessResult<Address>) result;
            
            Assert.NotNull(success.Result);
        }

        [Fact]
        public async Task ListAsyncShouldReturnAListOfAddresses()
        {
            const int total = 8;
            using var scope = _serviceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await context.Addresses.AddAsync(GetDummyAddress(Guid.NewGuid()));
            await context.SaveChangesAsync();

            var result = await sut.ListAsync(new PaginationData
            {
                CurrentPage = 1,
                PerPage = 10
            });
            var successResult = (SuccessResult<PaginationResult<Address>>) result;
            var pagination = successResult.Result;
            
            Assert.NotEmpty(pagination.Elements);
            Assert.Equal(total, pagination.Total);
        }

    }
}