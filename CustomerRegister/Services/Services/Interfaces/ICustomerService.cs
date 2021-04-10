using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;

namespace Services.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IServiceResult> SaveAsync(Customer customer);
    }
}