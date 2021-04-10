using System;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures;
using Services.DataStructures.Structs;
using Services.Repositories;
using Services.Services.Interfaces;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Services
{
    public class LocalPhoneServiceTests : DatabaseTestsBase
    {
        [Fact]
        public async Task SaveShouldANewEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var address = AddressFixture.GetDummyAddress(Guid.NewGuid());
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await sut.SaveAsync(LocalPhoneFixture.GetDummyLocalPhone(address.Id), address.Uuid);
            await unitOfWork.SaveChangesAsync();

            var phone = await context.LocalPhones.Include(x => x.PhoneAddress).FirstAsync();

            Assert.True(result.IsSuccessful);
            Assert.Equal(LocalPhoneFixture.Number, phone.Number);
            Assert.Equal(LocalPhoneFixture.AreaCode, phone.AreaCode);
            Assert.NotNull(phone.PhoneAddress);
        }

        [Fact]
        public async Task DetailAsyncShouldDetailAnExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var phoneUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(Guid.NewGuid());
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await context.LocalPhones.AddAsync(LocalPhoneFixture.GetDummyLocalPhone(phoneUuid, address.Id));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await sut.DetailAsync(phoneUuid);
            var successResult = (SuccessResult<LocalPhone>) result;
            var insertedPhone = successResult.Result;
            var insertedAddress = insertedPhone.PhoneAddress;

            Assert.Equal(LocalPhoneFixture.Number, insertedPhone.Number);
            Assert.Equal(LocalPhoneFixture.AreaCode, insertedPhone.AreaCode);
            Assert.Equal(AddressFixture.Country, insertedAddress.Country);
            Assert.Equal(AddressFixture.ZipCode, insertedAddress.ZipCode);
            Assert.Equal(AddressFixture.City, insertedAddress.City);
            Assert.Equal(AddressFixture.Number, insertedAddress.Number);
            Assert.Equal(AddressFixture.State, insertedAddress.State);
            Assert.Equal(AddressFixture.Street, insertedAddress.Street);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnNotFoundResultGivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();

            Assert.IsType<NotFoundResult>(await sut.DetailAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task ListAsyncShouldReturnAListOfEntries()
        {
            const int total = 5;
            const int perPage = 8;

            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(addressUuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            for (var i = 0; i < total; i++) await context.LocalPhones.AddAsync(LocalPhoneFixture.GetDummyLocalPhone(Guid.NewGuid(), address.Id));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await sut.ListAsync(new PaginationData
            {
                CurrentPage = 1,
                PerPage = perPage
            });
            var successResult = (SuccessResult<PaginationResult<LocalPhone>>) result;
            var phones = successResult.Result.Elements;
            foreach (var phone in phones)
            {
                Assert.Equal(LocalPhoneFixture.Number, phone.Number);
                Assert.Equal(LocalPhoneFixture.AreaCode, phone.AreaCode);
                Assert.Equal(addressUuid, phone.PhoneAddress.Uuid);
            }
        }

        [Fact]
        public async Task SaveShouldReturnNotFoundResultGivenNonExistingPhoneUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(addressUuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var dummyPhone = new LocalPhone
            {
                Uuid = Guid.NewGuid()
            };

            var result = await sut.SaveAsync(dummyPhone, addressUuid);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveShouldReturnNotFoundResultGivenNonExistingAddressUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var (address, phone) = await SeedDatabaseFixture.AddPhoneAndAddressAsync(context);

            var phoneToEdit = new LocalPhone
            {
                Id = phone.Id,
                Uuid = phone.Uuid,
                Number = "4578-8795",
                AreaCode = "84"
            };

            var result = await sut.SaveAsync(phoneToEdit, Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveShouldUpdateGivenExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var (address, phone) = await SeedDatabaseFixture.AddPhoneAndAddressAsync(context);

            var newAddressUuid = Guid.NewGuid();
            var newAddress = AddressFixture.GetDummyAddress(newAddressUuid);
            await context.Addresses.AddAsync(newAddress);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            
            const string newNumber = "4578-8795";
            const string newAreaCode = "84";
            var phoneToEdit = new LocalPhone
            {
                Id = phone.Id,
                Uuid = phone.Uuid,
                Number = newNumber,
                AreaCode = newAreaCode
            };

            var result = await sut.SaveAsync(phoneToEdit, newAddress.Uuid);
            await unitOfWork.SaveChangesAsync();
            var newPhone = await context.LocalPhones.Include(x => x.PhoneAddress).FirstAsync(x => x.Uuid == phoneToEdit.Uuid);
            
            Assert.True(result.IsSuccessful);
            Assert.Equal(phone.Uuid, newPhone.Uuid);
            Assert.Equal(phone.Id, newPhone.Id);
            Assert.Equal(newAddressUuid, newPhone.PhoneAddress.Uuid);
            Assert.NotEqual(address.Uuid, newPhone.PhoneAddress.Uuid);
            Assert.Equal(newNumber, newPhone.Number);
            Assert.Equal(newAreaCode, newPhone.AreaCode);
        }



        [Fact]
        public async Task DeleteAsyncShouldSetIsActiveToFalse()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var (_, phone) = await SeedDatabaseFixture.AddPhoneAndAddressAsync(context);

            var result = await sut.DeleteAsync(phone.Uuid);
            await unitOfWork.SaveChangesAsync();

            var deletedPhone = await context.LocalPhones.FirstAsync();
            
            Assert.True(result.IsSuccessful);
            Assert.Equal(phone.Uuid, deletedPhone.Uuid);
            Assert.False(deletedPhone.IsActive);
        }
    }
}