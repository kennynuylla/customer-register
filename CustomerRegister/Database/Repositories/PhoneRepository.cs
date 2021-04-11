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
    internal class PhoneRepository : RepositoryBase<Phone>, IPhoneRepository
    {
        public PhoneRepository(ApplicationContext context, ILogger<PhoneRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Guid>> GetUuidsFromCustomer(Guid customerUuid) =>
            await Set.AsNoTracking().Where(x => x.Customer.Uuid == customerUuid).Select(x => x.Uuid).ToListAsync();

        public async Task<IEnumerable<Phone>> GetPhonesFromCustomerAsync(Guid customerUuid) => await
            Set.AsNoTracking().Where(x => x.Customer.Uuid == customerUuid && x.IsActive)
                .ToListAsync();
    }
}