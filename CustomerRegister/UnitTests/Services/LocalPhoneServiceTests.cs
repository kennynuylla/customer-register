using System;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        
    }
}