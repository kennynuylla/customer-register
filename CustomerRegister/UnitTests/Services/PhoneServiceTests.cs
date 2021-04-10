using System;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures;
using Services.Services.Interfaces;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Services
{
    public class PhoneServiceTests : DatabaseTestsBase
    {
        [Fact]
        public async Task SaveAsyncShouldReturnNotFoundGivenNonExistingCustomerUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var (_, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);

            var editedPhone = new Phone
            {
                Id = phone.Id,
                Uuid = phone.Uuid,
                Number = "7894-8578",
                CustomerId = phone.CustomerId,
                AreaCode = "87"
            };

            var result = await sut.SaveAsync(editedPhone, Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveAsyncShouldReturnNotFoundResultGivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var (customer, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);

            var editedPhone = new Phone
            {
                Id = 3587,
                Uuid = phone.Uuid,
                Number = "7894-8578",
                CustomerId = phone.CustomerId,
                AreaCode = "87"
            };

            var result = await sut.SaveAsync(editedPhone, customer.Uuid);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveAsyncShouldAddNewPhoneGivenNonExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);
            var phone = new Phone
            {
                Number = PhoneFixture.Number,
                AreaCode = PhoneFixture.AreaCode
            };
            var result = await sut.SaveAsync(phone, customer.Uuid);
            await unitOfWork.SaveChangesAsync();
            var savedPhone = await context.Phones.Include(x => x.Customer).FirstAsync();
            
            Assert.IsType<SuccessResult<Guid>>(result);
            Assert.NotEqual(default, savedPhone.Id);
            Assert.NotEqual(default, savedPhone.Uuid);
            Assert.Equal(customer.Id, savedPhone.CustomerId);
            Assert.Equal(customer.Uuid, savedPhone.Customer.Uuid);
            Assert.Equal(customer.Name, savedPhone.Customer.Name);
            Assert.Equal(customer.Email, savedPhone.Customer.Email);
            Assert.Equal(PhoneFixture.Number, savedPhone.Number);
            Assert.Equal(PhoneFixture.AreaCode, savedPhone.AreaCode);
        }

        [Fact]
        public async Task SaveAsyncShouldUpdateExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var (_, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            var editPhone = new Phone
            {
                Id = phone.Id,
                Uuid = phone.Uuid,
                Number = "4587-9963",
                AreaCode = "78"
            };
            var result = await sut.SaveAsync(editPhone, customer.Uuid);
            await unitOfWork.SaveChangesAsync();
            var savedPhone = await context.Phones.Include(x => x.Customer).FirstAsync();

            Assert.IsType<SuccessResult<Guid>>(result);
            Assert.NotEqual(default, savedPhone.Id);
            Assert.NotEqual(default, savedPhone.Uuid);
            Assert.Equal(customer.Id, savedPhone.CustomerId);
            Assert.Equal(customer.Uuid, savedPhone.Customer.Uuid);
            Assert.Equal(customer.Name, savedPhone.Customer.Name);
            Assert.Equal(customer.Email, savedPhone.Customer.Email);
            Assert.Equal(editPhone.Number, savedPhone.Number);
            Assert.Equal(savedPhone.AreaCode, savedPhone.AreaCode);
        }
    }
}