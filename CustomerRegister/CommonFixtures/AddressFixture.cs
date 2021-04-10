using System;
using Domain.Models;

namespace CommonFixtures
{
    public static class AddressFixture
    {
        public const string City = "Palmas";
        public const string ZipCode = "77001-004";
        public const string Country = "Brazil";
        public const string State = "Tocantins";
        public const string Street = "Non Existing Street";
        public const int Number = 70;
        
        public static Address GetDummyAddress()
        {
            return new Address
            {
                City = City,
                Country = Country,
                State = State,
                ZipCode = ZipCode,
                Street = Street,
                Number = Number
            };
        }
        
        public static Address GetDummyAddress(Guid uuid)
        {
            var address = GetDummyAddress();
            address.Uuid = uuid;

            return address;
        }
    }
}