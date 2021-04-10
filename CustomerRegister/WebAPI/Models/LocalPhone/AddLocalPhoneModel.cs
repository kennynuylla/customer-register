using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.LocalPhone
{
    public class AddLocalPhoneModel
    {
        [Required]
        public string Number { get; set; }
        
        [Required]
        public string AreaCode { get; set; }
        
        [Required]
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