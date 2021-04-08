using System;
using Domain.Enums;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Phone : PhoneBase
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}