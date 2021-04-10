using System;

namespace WebAPI.Models.LocalPhone
{
    public class LocalPhoneListItemModel
    {
        public string Number { get; set; }
        public string AreaCode { get; set; }
        public Guid AddressUuid { get; set; }
        public string AddressDescription { get; set; }

        public LocalPhoneListItemModel()
        {
            
        }

        public LocalPhoneListItemModel(Domain.Models.LocalPhone phone)
        {
            Number = phone.Number;
            AreaCode = phone.AreaCode;
            AddressUuid = phone.PhoneAddress.Uuid;
            AddressDescription = $"{phone.PhoneAddress.Street}, {phone.PhoneAddress.City}, " +
                                 $"{phone.PhoneAddress.ZipCode}, {phone.PhoneAddress.State}, {phone.PhoneAddress.Country}.";
        }
    }
}