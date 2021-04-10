using System;
using Domain.Models;

namespace CommonFixtures
{
    public static class PhoneFixture
    {
        public const string Number = "1547-8975";
        public const string AreaCode = "74";

        public static Phone GetDummyPhone(int customerId) => new Phone
        {
            Number = Number,
            AreaCode = AreaCode,
            CustomerId = customerId
        };

        public static Phone GetDummyPhone(int customerId, Guid uuid)
        {
            var phone = GetDummyPhone(customerId);
            phone.Uuid = uuid;

            return phone;
        }
    }
}