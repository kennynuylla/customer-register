using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;

namespace Services.Services.Interfaces
{
    public interface IAddressService
    {
        IServiceResult Save(Address address);
    }
}