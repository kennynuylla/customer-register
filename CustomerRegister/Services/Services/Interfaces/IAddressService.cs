using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IServiceResult> ListAsync(PaginationData pagination);
        Task<IServiceResult> DetailAsync(Guid uuid); 
        IServiceResult Save(Address address);
    }
}