﻿using System;
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
using WebAPI.Models.Phone;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class PhoneControllerTests : ControllerTestsBase
    {
        public PhoneControllerTests(ApplicationFactory factory) : base(factory)
        {
        }
        
        private static void AssertPhone(Phone phoneToAssert,Customer customer, Phone phoneWithExpectedData)
        {
            Assert.NotNull(phoneToAssert);
            Assert.Equal(phoneWithExpectedData.Number, phoneToAssert.Number);
            Assert.Equal(phoneWithExpectedData.AreaCode, phoneToAssert.AreaCode);
            Assert.Equal(customer.Uuid, phoneToAssert.Customer.Uuid);
            Assert.Equal(customer.Id, phoneToAssert.Customer.Id);
            Assert.Equal(customer.Name, phoneToAssert.Customer.Name);
            Assert.Equal(customer.Email, phoneToAssert.Customer.Email);
        }

        [Fact]
        public async Task AddShouldAddNewPhone()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var customer = await SeedDatabaseFixture.AddDummyCustomerAsync(context);

            var request = new AddPhoneModel
            {
                Number = "7894-5698",
                AreaCode = "78",
                CustomerUuid = customer.Uuid
            };
            var serializedRequest = JsonSerializer.Serialize(request);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PostAsync("Phone/Add", contentRequest);
            result.EnsureSuccessStatusCode();
            var savedPhone = await context.Phones.Include(x => x.Customer).FirstAsync();
            
            Assert.Equal(request.Number, savedPhone.Number);
            Assert.Equal(request.AreaCode, savedPhone.AreaCode);
            Assert.Equal(request.CustomerUuid, savedPhone.Customer.Uuid);
        }

        [Fact]
        public async Task GetShouldReturn404GivenNonExistingPhone()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();

            var result = await sut.GetAsync($"Phone/Get/{Guid.NewGuid()}");
            
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetShouldReturnDetailsGivenExistingPhone()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var (customer, phone) = await SeedDatabaseFixture.AddDummyCustomerAndPhoneAsync(context);

            var result = await sut.GetAsync($"Phone/Get/{phone.Uuid}");
            result.EnsureSuccessStatusCode();
            var serializedResult = await result.Content.ReadAsStringAsync();
            var deserializedResult = JsonSerializer.Deserialize<Phone>(serializedResult, scope.ServiceProvider.GetRequiredService<JsonSerializerOptions>());
            
            AssertPhone(deserializedResult, customer, phone);
        }


    }
}