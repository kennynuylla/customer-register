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
    public class CustomerServiceTests : DatabaseTestsBase
    {

        [Fact]
        public async Task SaveShouldAddANewCustomer()
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
        public async Task SaveShouldEditExistingEntry()
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
        public async Task SaveShouldReturnNotFoundResultGivenNonExistingUuid()
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
    }
}