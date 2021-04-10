using System;
using Domain.Models;

namespace CommonFixtures
{
    public static class CustomerFixture
    {
        public const string Name = "Cícero";
        public static Customer GetDummyCustomer() => new Customer
        {
            Email = Guid.NewGuid().ToString(),
            Name = Name
        };

        public static Customer GetDummyCustomer(Guid uuid)
        {
            var customer = GetDummyCustomer();
            customer.Uuid = uuid;

            return customer;
        }
    }
}