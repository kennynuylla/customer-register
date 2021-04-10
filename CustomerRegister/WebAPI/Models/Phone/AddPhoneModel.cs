using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Phone
{
    public class AddPhoneModel
    {
        [Required]
        public string Number { get; set; }
        
        [Required]
        public string AreaCode { get; set; }
        
        [Required]
        public Guid CustomerUuid { get; set; }

        public (Domain.Models.Phone phone, Guid customerUuid) GetPhone() => (new Domain.Models.Phone
        {
            Number = Number,
            AreaCode = AreaCode
        }, CustomerUuid);
    }
}