using System;
using System.ComponentModel;
using Domain.Enums;
using Domain.Models.Interfaces;

namespace Domain.Models.Base
{
    public abstract class PhoneBase : IBaseModel
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public PhoneType Type { get; set; }
        public bool IsActive { get; set; }

        public PhoneBase()
        {
            IsActive = true;
        }
    }
}