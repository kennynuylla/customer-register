using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;

namespace Services.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IServiceResult> DetailAsync(Guid uuid); 
        IServiceResult Save(Address address);
    }
}