using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;

namespace Services.Services.Interfaces
{
    public interface ILocalPhoneService
    {
        Task<IServiceResult> SaveAsync(LocalPhone phone, Guid addressUuid);
        Task<IServiceResult> DetailAsync(Guid uuid);
    }
}