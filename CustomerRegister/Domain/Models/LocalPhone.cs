using Domain.Models.Base;

namespace Domain.Models
{
    public class LocalPhone : PhoneBase
    {
        public int PhoneAddressId{ get; set; }
        public Address PhoneAddress { get; set; }
    }
}