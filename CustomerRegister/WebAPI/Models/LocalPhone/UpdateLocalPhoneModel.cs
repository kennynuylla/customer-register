using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.LocalPhone
{
    public class UpdateLocalPhoneModel : AddLocalPhoneModel
    {
        [RegularExpression("[1-9]*")]
        public int Id { get; set; }

        public (Domain.Models.LocalPhone phone, Guid addressGuid) GetPhone(Guid uuid)
        {
            var (phone, addressGuid) = base.GetPhone();
            phone.Id = Id;
            phone.Uuid = uuid;

            return (phone, addressGuid);
        }
    }
}