using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Repositories
{
    public interface ILocalPhoneRepository : IRepositoryBase<LocalPhone>
    {
        Task<IEnumerable<Guid>> GetGuidsFromAddress(Guid addressUuid);
        Task<IEnumerable<LocalPhone>> GetFromAddressAsync(Guid addressUuid);
    }
}