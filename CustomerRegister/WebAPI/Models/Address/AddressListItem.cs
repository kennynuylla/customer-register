using System;

namespace WebAPI.Models.Address
{
    public class AddressListItem
    {
        public Guid Uuid { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public AddressListItem()
        {
            
        }
        public AddressListItem(Domain.Models.Address address)
        {
            Uuid = address.Uuid;
            Street = address.Street;
            Number = address.Number;
            ZipCode = address.ZipCode;
            City = address.City;
            State = address.State;
            Country = address.Country;
        }
    }
}