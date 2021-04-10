using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Address
{
    public class AddAddressModel
    {
        [Required]
        public string Street { get; set; }
        
        [RegularExpression("[1-9][0-9]*")]
        public int Number { get; set; }
        
        [Required]
        public string ZipCode { get; set; }
        
        [Required]
        public string City { get; set; }
        
        [Required]
        public string State { get; set; }
        
        [Required]
        public string Country { get; set; }

        public Domain.Models.Address GetAddress() => new Domain.Models.Address
        {
            City = City,
            Country = Country,
            Number = Number,
            State = State,
            Street = Street,
            ZipCode = ZipCode
        };
    }
}