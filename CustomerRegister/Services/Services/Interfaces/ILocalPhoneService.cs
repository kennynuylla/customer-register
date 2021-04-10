using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Services.Interfaces
{
    public interface ILocalPhoneService
    {
        Task<IServiceResult> SaveAsync(LocalPhone phone, Guid addressUuid);
        Task<IServiceResult> DetailAsync(Guid uuid);
        Task<IServiceResult> ListAsync(PaginationData pagination);
    }
}