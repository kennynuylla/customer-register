using System;

namespace Domain.Models.Interfaces
{
    public interface IBaseModel
    {
        public Guid Uuid { get; set; }
        public bool IsActive { get; set; }
    }
}