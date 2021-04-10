using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Domain.Models.Interfaces;

namespace Domain.Models
{
    public class Customer : IBaseModel
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Address> Addresses { get; set; }

        public Customer()
        {
            IsActive = true;
        }
    }
}