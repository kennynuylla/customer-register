using Database.Repositories.Base;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Database.Repositories
{
    internal class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository 
    {
        public CustomerRepository(ApplicationContext context, ILogger<CustomerRepository> logger) : base(context, logger)
        {
        }
    }
}