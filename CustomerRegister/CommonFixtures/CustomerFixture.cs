using System;
using Domain.Models;

namespace CommonFixtures
{
    public static class CustomerFixture
    {
        public const string Name = "Cícero";

        public static Customer GetDummyCustomer() => GetDummyCustomer(Guid.NewGuid().ToString());
        public static Customer GetDummyCustomer(string email) => new Customer
        {
            Email = email,
            Name = Name
        };

        public static Customer GetDummyCustomer(Guid uuid, string email)
        {
            var customer = GetDummyCustomer(email);
            customer.Uuid = uuid;

            return customer;
        }
    }
}