using System;
using System.Threading.Tasks;
using Database.Repositories.Base;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Database.Repositories
{
    internal class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository 
    {
        public CustomerRepository(ApplicationContext context, ILogger<CustomerRepository> logger) : base(context, logger)
        {
        }

        public Task<Customer> GetCustomerWithAddress(Guid uuid) => Set.AsNoTracking()
            .Include(x => x.Addresses)
            .FirstAsync(x => x.Uuid == uuid && x.IsActive);

        public async Task<bool> CheckEmailAlreadyRegisteredAsync(string email) => await Set.AnyAsync(x => x.Email == email);
    }
}