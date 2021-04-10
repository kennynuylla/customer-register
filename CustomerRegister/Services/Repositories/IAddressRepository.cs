using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Repositories
{
    public interface IAddressRepository : IRepositoryBase<Address>
    {
        public Task<IEnumerable<Address>> GetAddressesFromCustomer(Guid uuid);
        public Task<Address> GetTrackedAsync(Guid uuid);
    }
}