using System;

namespace WebAPI.Models.Phone
{
    public class UpdatePhoneModel : AddPhoneModel
    {
        public int Id { get; set; }

        public (Domain.Models.Phone phone, Guid customerUuid) GetPhone(Guid uuid)
        {
            var (phone, customerUuid) = base.GetPhone();
            phone.Id = Id;
            phone.Uuid = uuid;

            return (phone, customerUuid);
        }
    }
}