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
        
        private static void AssertPhone(Phone phoneFromContext, Customer customer, Phone phone)
        {
            Assert.NotEqual(default, phoneFromContext.Id);
            Assert.NotEqual(default, phoneFromContext.Uuid);
            Assert.Equal(customer.Id, phoneFromContext.CustomerId);
            Assert.Equal(customer.Uuid, phoneFromContext.Customer.Uuid);
            Assert.Equal(customer.Name, phoneFromContext.Customer.Name);
            Assert.Equal(customer.Email, phoneFromContext.Customer.Email);
            Assert.Equal(phone.Number, phoneFromContext.Number);
            Assert.Equal(phoneFromContext.AreaCode, phoneFromContext.AreaCode);
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
            AssertPhone(savedPhone, customer, phone);
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
            AssertPhone(savedPhone, customer, editPhone);
        }
        
        [Fact]
        public async Task DetailAsyncShouldReturnTheDetailsOfAnEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var (customer, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);

            var result = await sut.DetailAsync(phone.Uuid);
            var successResult = (SuccessResult<Phone>) result;
            var detailedPhone = successResult.Result;
            
            AssertPhone(detailedPhone, customer, phone);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnNotFoundResultGivenNonExitingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            
            var result = await sut.DetailAsync(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ListAsyncShouldReturnAListOfEntries()
        {
            const int total = 5;
            const int perPage = 17;
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);
            for (var i = 0; i < total; i++) await SeedDatabaseFixture.AddDummyPhoneAsync(context, customer);

            var result = await sut.ListAsync(new PaginationData
            {
                CurrentPage = 1,
                PerPage = perPage
            });
            var successResult = (SuccessResult<PaginationResult<Phone>>) result;
            foreach (var phone in successResult.Result.Elements)
            {
                Assert.Equal(PhoneFixture.Number, phone.Number);
                Assert.Equal(PhoneFixture.AreaCode, phone.AreaCode);
                Assert.Equal(customer.Id, phone.CustomerId);
                Assert.Equal(customer.Uuid, phone.Customer.Uuid);
            }
        }

        [Fact]
        public async Task DeleteAsyncShouldSetIsActiveToFalse()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<IPhoneService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var (_, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);

            var result = await sut.DeleteAsync(phone.Uuid);
            var deletedPhone = await context.Phones.FirstAsync();
            
            Assert.True(result.IsSuccessful);
            Assert.False(deletedPhone.IsActive);
            Assert.NotEqual(default, deletedPhone.Uuid);
        }
    }
}