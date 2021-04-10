using System;
using System.Threading.Tasks;
using Database;
using Domain.Models;

namespace CommonFixtures
{
    public static class SeedDatabaseFixture
    {
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