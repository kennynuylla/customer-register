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
    internal class LocalPhoneRepository : RepositoryBase<LocalPhone>, ILocalPhoneRepository
    {
        public LocalPhoneRepository(ApplicationContext context, ILogger<LocalPhoneRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Guid>> GetGuidsFromAddress(Guid addressUuid) =>
            await Set
                .AsNoTracking()
                .Where(x => x.PhoneAddress.Uuid == addressUuid)
                .Select(x => x.Uuid)
                .ToListAsync();
    }
}