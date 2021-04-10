using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IServiceResult> SaveAsync(Customer customer);
        Task<IServiceResult> DetailAsync(Guid uuid);
        Task<IServiceResult> ListAsync(PaginationData pagination);
    }
}