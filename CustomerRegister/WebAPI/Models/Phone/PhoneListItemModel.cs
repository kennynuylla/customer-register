using System;

namespace WebAPI.Models.Phone
{
    public class PhoneListItemModel
    {
        public string Number { get; set; }
        public string AreaCode { get; set; }
        public Guid Uuid { get; set; }
        public string CustomerName { get; set; }

        public PhoneListItemModel()
        {
            
        }

        public PhoneListItemModel(Domain.Models.Phone phone)
        {
            Number = phone.Number;
            AreaCode = phone.AreaCode;
            Uuid = phone.Uuid;
            CustomerName = phone.Customer.Name;
        }
    }
}