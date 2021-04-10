namespace WebAPI.Models.Customer
{
    public class AddCustomerModel
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public Domain.Models.Customer GetCustomer() => new Domain.Models.Customer
        {
            Email = Email,
            Name = Name
        };
    }
}