using System.Collections.Generic;
using Domain.Models;

namespace Services.DataStructures
{
    public class CustomerContainer
    {
        public Customer Customer { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Phone> Phones { get; set; }
        public IEnumerable<LocalPhone> LocalPhones { get; set; }
        public IEnumerable<Customer> Roomates { get; set; }

        public CustomerContainer()
        {
            Addresses = new List<Address>();
            Phones = new List<Phone>();
            LocalPhones = new List<LocalPhone>();
            Roomates = new List<Customer>();
        }
    }
}