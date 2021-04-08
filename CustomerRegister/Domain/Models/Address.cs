using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Models.Interfaces;

namespace Domain.Models
{
    public class Address : IUuidModel
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
    }
}