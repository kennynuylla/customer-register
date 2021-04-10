using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IServiceResult> SaveAsync(Customer customer, IEnumerable<Guid> addresses);
        Task<IServiceResult> DetailAsync(Guid uuid);
        Task<IServiceResult> ListAsync(PaginationData pagination);
        Task<IServiceResult> DeleteAsync(Guid uuid);
    }
}