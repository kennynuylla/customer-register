using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Repositories
{
    public interface IPhoneRepository : IRepositoryBase<Phone>
    {
        Task<IEnumerable<Guid>> GetUuidsFromCustomer(Guid customerUuid);
    }
}