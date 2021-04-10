using System.Threading.Tasks;
using Domain.Models;

namespace Services.Repositories
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        Task<bool> CheckEmailAlreadyRegisteredAsync(string email);
    }
}