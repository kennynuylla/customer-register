using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommonFixtures;
using Database;
using IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Models.LocalPhone;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class LocalPhoneControllerTests : ControllerTestsBase
    {
        public LocalPhoneControllerTests(ApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task AddShouldAddANewEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = Factory.CreateClient();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var addressUuid = Guid.NewGuid();
            await context.Addresses.AddAsync(AddressFixture.GetDummyAddress(addressUuid));
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var request = new AddLocalPhoneModel
            {
                Number = LocalPhoneFixture.Number,
                AreaCode = LocalPhoneFixture.AreaCode,
                AddressUuid = addressUuid
            };

            var serializedRequest = JsonSerializer.Serialize(request);
            var contentRequest = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var result = await sut.PostAsync("LocalPhone/Add", contentRequest);
            result.EnsureSuccessStatusCode();

            var insertedPhone = await context.LocalPhones.Include(x => x.PhoneAddress).FirstAsync();
            
            Assert.Equal(LocalPhoneFixture.Number, insertedPhone.Number);
            Assert.Equal(LocalPhoneFixture.AreaCode, insertedPhone.AreaCode);
            Assert.NotNull(insertedPhone.PhoneAddress);
        }
    }
}