using System;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures;
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

            var result =  await sut.SaveAsync(LocalPhoneFixture.GetDummyLocalPhone(address.Id), address.Uuid);
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
        
    }
}