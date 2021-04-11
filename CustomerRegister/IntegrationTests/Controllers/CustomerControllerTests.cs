using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using Domain.Models;
using IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.DataStructures;
using Services.DataStructures.Structs;
using WebAPI.Models.Customer;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class CustomerControllerTests : ControllerTestsBase
    {
        public CustomerControllerTests(ApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AddShouldCreateANewCustomer()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var customerToAdd = new AddCustomerModel
            {
                Email = "cicero@customer.com",
                Name = CustomerFixture.Name
            };
            var serializedRequest = JsonSerializer.Serialize(customerToAdd);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PostAsync("Customer/Add", contentRequest);
            result.EnsureSuccessStatusCode();

            var insertedCustomer = await context.Customers.FirstAsync();
            Assert.Equal(customerToAdd.Email, insertedCustomer.Email);
            Assert.Equal(customerToAdd.Name, insertedCustomer.Name);
            Assert.NotEqual(default, insertedCustomer.Uuid);
        }

        [Fact]
        public async Task GetShouldDetailACustomer()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            var result = await sut.GetAsync($"Customer/Get/{customer.Uuid}");
            result.EnsureSuccessStatusCode();
            var serializedResult = await result.Content.ReadAsStringAsync();
            var detailedCustomer = JsonSerializer.Deserialize<DetailCustomerModel>(serializedResult, scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());
            
            Assert.NotNull(detailedCustomer);
            Assert.Equal(customer.Email, detailedCustomer.Email);
            Assert.Equal(customer.Name, detailedCustomer.Name);
            Assert.Equal(customer.Uuid, detailedCustomer.Uuid);
            Assert.Equal(customer.Id, detailedCustomer.Id);
            Assert.NotNull(detailedCustomer.Addresses);
            Assert.NotNull(detailedCustomer.Phones);
            Assert.NotNull(detailedCustomer.LocalPhones);
            Assert.NotNull(detailedCustomer.Roomates);
        }

        [Fact]
        public async Task GetShouldReturn404GivenNonExistingUuid()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            
            var result  = await sut.GetAsync($"Customer/Get/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ListShouldReturnAListOfCustomer()
        {
            const int total = 13;
            const int perPage = 20;
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            for (var i = 0; i < total; i++) await SeedDatabaseFixture.AddDummyCustomerAsync(context);
            var result = await sut.GetAsync($"Customer/List?currentPage=1&perPAge={perPage}");
            result.EnsureSuccessStatusCode();
            var serializedResult = await result.Content.ReadAsStringAsync();
            var deserializedResult = JsonSerializer.Deserialize<PaginationResult<CustomerListItemModel>>(serializedResult,
                scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());
            var customers = deserializedResult.Elements;
            foreach (var customer in customers)
            {
                Assert.NotEqual(default, customer.Uuid);
                Assert.Equal(CustomerFixture.Name, customer.Name);
            }
        }

        [Fact]
        public async Task UpdateShouldUpdateAnExistingCustomer()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);
            var editRequest = new UpdateCustomerModel
            {
                Email = "updated@test.com",
                Id = customer.Id,
                Name = "Charlie"
            };
            var serializedRequest = JsonSerializer.Serialize(editRequest);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PutAsync($"Customer/Update/{customer.Uuid}", contentRequest);
            result.EnsureSuccessStatusCode();
            var savedCustomer = await context.Customers.FirstAsync();
            
            Assert.Equal(customer.Uuid, savedCustomer.Uuid);
            Assert.Equal(customer.Id, savedCustomer.Id);
            Assert.Equal(editRequest.Email, savedCustomer.Email);
            Assert.Equal(editRequest.Name, savedCustomer.Name);
        }

        [Fact]
        public async Task DeleteShouldSeIsActiveToFalse()
        {
               using var scope = ServiceProvider.CreateScope();
               var sut = Factory.CreateClient();
               var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
               var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

               var result = await sut.DeleteAsync($"Customer/Delete/{customer.Uuid}");
               result.EnsureSuccessStatusCode();
               var deletedCustomer = await context.Customers.FirstAsync();
               
               Assert.False(deletedCustomer.IsActive);
        }
    }
}