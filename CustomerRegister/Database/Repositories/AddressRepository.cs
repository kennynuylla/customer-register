using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Repositories.Base;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Database.Repositories
{
    internal class AddressRepository : RepositoryBase<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationContext context, ILogger<AddressRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Address>> GetAddressesFromCustomer(Guid uuid) => await Set.AsNoTracking()
            .Where(x => x.Customers.Any(x => x.Uuid == uuid))
            .ToListAsync();

        public async Task<Address> GetTrackedAsync(Guid uuid) => await Set.FirstAsync(x => x.Uuid == uuid);
    }
}