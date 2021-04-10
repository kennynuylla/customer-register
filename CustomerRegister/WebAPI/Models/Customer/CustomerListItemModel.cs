using System;

namespace WebAPI.Models.Customer
{
    public class CustomerListItemModel
    {
        public string Name { get; set; }
        public Guid Uuid { get; set; }

        public CustomerListItemModel()
        {
            
        }

        public CustomerListItemModel(Domain.Models.Customer customer)
        {
            Name = customer.Name;
            Uuid = customer.Uuid;
        }
    }
}