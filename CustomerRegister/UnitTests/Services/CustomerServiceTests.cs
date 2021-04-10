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
    public class CustomerServiceTests : DatabaseTestsBase
    {

        [Fact]
        public async Task SaveAsyncShouldAddANewCustomer()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var result = await sut.SaveAsync(CustomerFixture.GetDummyCustomer());
            await unitOfWork.SaveChangesAsync();

            var customer = await context.Customers.FirstAsync();
            
            Assert.True(result.IsSuccessful);
            Assert.Equal(CustomerFixture.Name, customer.Name);
            Assert.NotEqual(default, customer.Email);
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
            var result = await sut.SaveAsync(editedCustomer);
            await unitOfWork.SaveChangesAsync();
            
            var savedCustomer = await context.Customers.FirstAsync();
            Assert.True(result.IsSuccessful);
            Assert.Equal(editedEmail, savedCustomer.Email);
            Assert.Equal(customer.Name, savedCustomer.Name);
            Assert.Equal(customer.Uuid, savedCustomer.Uuid);
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
            var result = await sut.SaveAsync(editedCustomer);
            Assert.IsType<NotFoundResult>(result);
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
            
            Assert.Equal(customer.Addresses, detailedCustomer.Addresses);
            Assert.Equal(customer.Name, detailedCustomer.Name);
            Assert.Equal(customer.Uuid, detailedCustomer.Uuid);
            Assert.Equal(customer.Id, detailedCustomer.Id);
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
    }
}