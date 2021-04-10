using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Domain.Models.Interfaces;

namespace Domain.Models
{
    public class Address : IBaseModel
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Customer> Customers { get; set; }

        public Address()
        {
            IsActive = true;
        }
    }
}