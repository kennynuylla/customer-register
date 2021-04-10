using System;
using System.Linq;
using System.Threading.Tasks;
using CommonFixtures;
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
    public class AddressServiceTests : DatabaseTestsBase
    {
        [Fact]
        public async Task SaveShouldAddANewAddressGivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var address = AddressFixture.GetDummyAddress();

            var result = await sut.SaveAsync(address);
            await unitOfWork.SaveChangesAsync();

            Assert.True(result.IsSuccessful);
            Assert.NotEmpty(context.Addresses);
            Assert.Equal(1,context.Addresses.Count());

        }
        
        [Fact]
        public async Task SaveShouldEditExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(uuid);
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

            var result = await sut.SaveAsync(editedAddress);
            await unitOfWork.SaveChangesAsync();
            var uniqueAddress = await context.Addresses.FirstAsync();
            
            Assert.True(result.IsSuccessful);
            Assert.Equal(edited, uniqueAddress.Country);
            Assert.Equal(uuid, uniqueAddress.Uuid);
        }

        [Fact]
        public async Task SaveShouldReturnNotFoundResultGivenNonExistingUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(uuid);
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
                Uuid = Guid.NewGuid(),
                ZipCode = address.ZipCode,
                State = address.State
            };

            var result = await sut.SaveAsync(editedAddress);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnNotFoundResultGivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();

            var result = await sut.DetailAsync(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnDataGivenExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(uuid);
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
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await context.Addresses.AddAsync(AddressFixture.GetDummyAddress(Guid.NewGuid()));
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

        [Fact]
        public async Task DeleteShouldSetIsActiveToFalseToAddressAndItsPhones()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IAddressService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
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

            await sut.DeleteAsync(addressUuid);
            await unitOfWork.SaveChangesAsync();

            var deletedAddress = await context.Addresses.FirstAsync(x => x.Uuid == addressUuid);
            var deletedPhone = await context.LocalPhones.FirstAsync(x => x.Uuid == phoneUuid);
            Assert.False(deletedAddress.IsActive);
            Assert.False(deletedPhone.IsActive);
        }

    }
}