using System;
using System.Linq;
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
    public class CustomerServiceTests : DatabaseTestsBase
    {

        [Fact]
        public async Task SaveAsyncShouldAddANewCustomer()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var address = await SeedDatabaseFixture.AddDummyAddressAsync(context);

            var result = await sut.SaveAsync(CustomerFixture.GetDummyCustomer(), new []{address.Uuid});
            await unitOfWork.SaveChangesAsync();

            var customer = await context.Customers.Include(x => x.Addresses).FirstAsync();
            
            Assert.True(result.IsSuccessful);
            Assert.Equal(CustomerFixture.Name, customer.Name);
            Assert.NotEqual(default, customer.Email);
            Assert.NotEmpty(customer.Addresses);
            Assert.Equal(AddressFixture.City, customer.Addresses.First().City);
        }

        [Fact]
        public async Task SaveAsyncShouldEditExistingEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            const string editedEmail = "edited@ok.com";
            var editedCustomer = new Customer
            {
                Email = editedEmail,
                Id = customer.Id,
                Uuid = customer.Uuid,
                Name = customer.Name
            };
            var result = await sut.SaveAsync(editedCustomer, customer.Addresses.Select(x => x.Uuid));
            await unitOfWork.SaveChangesAsync();
            
            var savedCustomer = await context.Customers.FirstAsync();
            Assert.True(result.IsSuccessful);
            Assert.Equal(editedEmail, savedCustomer.Email);
            Assert.Equal(customer.Name, savedCustomer.Name);
            Assert.Equal(customer.Uuid, savedCustomer.Uuid);
            Assert.Contains(customer.Addresses.First().Uuid, savedCustomer.Addresses.Select(x => x.Uuid));
        }

        [Fact]
        public async Task SaveAsyncShouldReturnNotFoundResultGivenNonExistingUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            const string editedEmail = "edited@ok.com";
            var editedCustomer = new Customer
            {
                Email = editedEmail,
                Id = customer.Id,
                Uuid = Guid.NewGuid(),
                Name = customer.Name
            };
            var result = await sut.SaveAsync(editedCustomer, customer.Addresses.Select(x => x.Uuid));
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveAsyncShouldReturnFailResultGivenExistingEmail()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            const string emailToRepeat = "email@teste.com";
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context, emailToRepeat);
            var customerWithRepeatedEmail = CustomerFixture.GetDummyCustomer(emailToRepeat);
            var result = await sut.SaveAsync(customerWithRepeatedEmail, null);
            Assert.IsType<FailResult>(result);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnNotFoundResultGivenNonExistingUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await sut.DetailAsync(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailAsyncShouldReturnTheEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            var result = await sut.DetailAsync(customer.Uuid);
            var successResult = (SuccessResult<Customer>) result;
            var detailedCustomer = successResult.Result;
            
            Assert.Equal(customer.Name, detailedCustomer.Name);
            Assert.Equal(customer.Uuid, detailedCustomer.Uuid);
            Assert.Equal(customer.Id, detailedCustomer.Id);
            Assert.NotEmpty(detailedCustomer.Addresses);
        }

        [Fact]
        public async Task ListAsyncShouldReturnAListOfCustomers()
        {
            const int total = 8;
            const int perPage = 2;
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await SeedDatabaseFixture.AddDummyCustomerAsync(context);
            var result = await sut.ListAsync(new PaginationData
            {
                CurrentPage = 1,
                PerPage = perPage
            });
            var successResult = (SuccessResult<PaginationResult<Customer>>) result;
            var customers = successResult.Result.Elements;

            foreach (var customer in customers)
            {
                Assert.Equal(CustomerFixture.Name, customer.Name);
                Assert.NotEqual(default, customer.Email);
                Assert.NotEqual(default, customer.Uuid);
                Assert.NotEqual(default, customer.Id);
            }
        }

        [Fact]
        public async Task DeleteAsyncShouldSetIsActiveToFalse()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var (customer, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);

            var result = await sut.DeleteAsync(customer.Uuid);
            var deletedCustomer = await context.Customers.FirstAsync();
            var deletedPhone = await context.Phones.FirstAsync();

            Assert.True(result.IsSuccessful);
            Assert.False(deletedCustomer.IsActive);
            Assert.False(deletedPhone.IsActive);
        }
    }
}