using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Services.Services.Interfaces;
using UnitTests.Base;
using Xunit;

namespace UnitTests.Services
{
    public class LocalPhoneServiceTests : DatabaseTestsBase
    {
        private static LocalPhone GetDummyLocalPhone(int addressId) => new LocalPhone
        {
            Number = "3363-8745",
            AreaCode = "63",
            PhoneAddressId = addressId
        };

        private static LocalPhone GetDummyLocalPhone(Guid uuid, int addressId)
        {
            var phone = GetDummyLocalPhone(addressId);
            phone.Uuid = uuid;
            
            return phone;
        }

        [Fact]
        public async Task SaveShouldANewEntry()
        {
            using var scope = ServiceProvider.CreateScope();
            var sut = scope.ServiceProvider.GetRequiredService<ILocalPhoneService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            throw new NotImplementedException();
        }
        
    }
}