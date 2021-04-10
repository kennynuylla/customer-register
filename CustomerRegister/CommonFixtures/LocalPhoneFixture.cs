using System;
using System.Threading.Tasks;
using Database;
using Domain.Models;

namespace CommonFixtures
{
    public static class LocalPhoneFixture
    {
        public const string Number = "3363-8745";
        public const string AreaCode = "63";
        public static LocalPhone GetDummyLocalPhone(int addressId) => new LocalPhone
        {
            Number =Number,
            AreaCode = AreaCode,
            PhoneAddressId = addressId
        };

        public static LocalPhone GetDummyLocalPhone(Guid uuid, int addressId)
        {
            var phone = GetDummyLocalPhone(addressId);
            phone.Uuid = uuid;
            
            return phone;
        }
        
        public static async Task<(Address address, LocalPhone phone)> AddPhoneAndAddressAsync(ApplicationContext context)
        {
            var addressUuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(addressUuid);
            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var phoneUuid = Guid.NewGuid();
            var phone = LocalPhoneFixture.GetDummyLocalPhone(phoneUuid, address.Id);
            await context.LocalPhones.AddAsync(phone);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            return (address, phone);
        }
    }
}