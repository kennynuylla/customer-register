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
        
      
    }
}