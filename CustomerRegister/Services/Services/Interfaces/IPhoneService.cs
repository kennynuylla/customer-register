using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Services.Interfaces
{
    public interface IPhoneService
    {
        Task<IServiceResult> SaveAsync(Phone phone, Guid customerUuid);
        Task<IServiceResult> DetailAsync(Guid uuid);
        Task<IServiceResult> ListAsync(PaginationData pagination);
        Task<IServiceResult> DeleteAsync(Guid uuid);
    }
}