using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Customer
{
    public class AddCustomerModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Email { get; set; }
        public IEnumerable<Guid> AddressUuids { get; set; }

        public AddCustomerModel()
        {
            AddressUuids = new List<Guid>();
        }

        public (Domain.Models.Customer  customer, IEnumerable<Guid> uuid) GetCustomer() => (new Domain.Models.Customer
        {
            Email = Email,
            Name = Name
        },AddressUuids);
    }
}