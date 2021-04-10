using System;
using System.Threading.Tasks;
using Database;
using Domain.Models;

namespace CommonFixtures
{
    public static class SeedDatabaseFixture
    {
        public static async Task<(Address address, LocalPhone phone)> AddDummyPhoneAndAddressAsync(ApplicationContext context)
        {
            var address = await AddDummyAddressAsync(context);
            var phone = await AddDummyPhoneAsync(context, address);

            return (address, phone);
        }

        public static async Task<Address> AddDummyAddressAsync(ApplicationContext context)
        {
            var uuid = Guid.NewGuid();
            var address = AddressFixture.GetDummyAddress(uuid);

            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            return address;
        }

        public static async Task<LocalPhone> AddDummyPhoneAsync(ApplicationContext context, Address address)
        {
            var phoneUuid = Guid.NewGuid();
            var phone = LocalPhoneFixture.GetDummyLocalPhone(phoneUuid, address.Id);
            await context.LocalPhones.AddAsync(phone);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            return phone;
        }

        public static async Task<Customer> AddDummyCustomerAsync(ApplicationContext context)
        {
            var customer = CustomerFixture.GetDummyCustomer(Guid.NewGuid());
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            return customer;
        }
    }
}