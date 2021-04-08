using System.Threading.Tasks;

namespace Database.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync();
    }
}