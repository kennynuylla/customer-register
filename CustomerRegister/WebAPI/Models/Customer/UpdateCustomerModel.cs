using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Customer
{
    public class UpdateCustomerModel : AddCustomerModel
    {
        [RegularExpression("[1-9]*")]
        public int Id { get; set; }



        public (Domain.Models.Customer GetCustomer, IEnumerable<Guid> addressesUuids) GetCustomer(Guid uuid)
        {
            var (customer, uuids) = base.GetCustomer();
            customer.Id = Id;
            customer.Uuid = uuid;

            return (customer,uuids);
        }
    }
}