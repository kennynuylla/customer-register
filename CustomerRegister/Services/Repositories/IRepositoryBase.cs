using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Repositories
{
    public interface IRepositoryBase<TModel> where TModel:IBaseModel
    {
        Task<PaginationResult<TModel>> ListAsync(PaginationData pagination, params Expression<Func<TModel, object>>[] includes);
        Task<TModel> GetAsync(Guid uuid, params Expression<Func<TModel, object>>[] includes);
        Task<Guid> SaveAsync(TModel model);
        Task DeleteAsync(Guid uuid);
        Task<bool> CheckExistenceAsync(Guid uuid);
    }
}