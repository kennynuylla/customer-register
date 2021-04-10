using System;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Address
{
    public class UpdateAddressModel : AddAddressModel
    {
        [RegularExpression("[1-9]*")]
        public int Id { get; set; }

        public Domain.Models.Address GetAddress(Guid uuid)
        {
            var address = base.GetAddress();
            address.Id = Id;
            address.Uuid = uuid;

            return address;
        }
    }
}