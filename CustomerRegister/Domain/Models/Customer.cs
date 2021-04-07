using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Interfaces;

namespace Domain.Models
{
    public class Customer : IUuidModel
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
    }
}