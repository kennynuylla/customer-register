using System;

namespace WebAPI.Models.LocalPhone
{
    public class AddLocalPhoneModel
    {
        public string Number { get; set; }
        public string AreaCode { get; set; }
        public Guid AddressUuid { get; set; }

        public (Domain.Models.LocalPhone phone, Guid addressGuid) GetPhone()
        {
            var phone = new Domain.Models.LocalPhone
            {
                Number = Number,
                AreaCode = AreaCode
            };

            return (phone, AddressUuid);
        }
    }
}