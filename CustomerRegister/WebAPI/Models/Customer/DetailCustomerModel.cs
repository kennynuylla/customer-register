using System;
using System.Collections.Generic;
using Services.DataStructures;

namespace WebAPI.Models.Customer
{
    public class DetailCustomerModel
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<Domain.Models.Address> Addresses { get; set; }
        public IEnumerable<Domain.Models.Phone> Phones { get; set; }
        public IEnumerable<Domain.Models.LocalPhone> LocalPhones { get; set; }
        public IEnumerable<Domain.Models.Customer> Roomates { get; set; }

        public DetailCustomerModel()
        {
            Addresses = new List<Domain.Models.Address>();
            Phones = new List<Domain.Models.Phone>();
            LocalPhones = new List<Domain.Models.LocalPhone>();
            Roomates = new List<Domain.Models.Customer>();
        }

        public DetailCustomerModel(CustomerContainer container)
        {
            Addresses = container.Addresses;
            Phones = container.Phones;
            LocalPhones =container. LocalPhones;
            Roomates = container.Roomates;
            Id = container.Customer.Id;
            Uuid = container.Customer.Uuid;
            Name = container.Customer.Name;
            Email = container.Customer.Email;
        }
    }
}