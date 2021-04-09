using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Repositories
{
    public interface IRepositoryBase<TModel> where TModel:IUuidModel
    {
        Task<PaginationResult<TModel>> ListAsync(PaginationData pagination, params Expression<Func<TModel, object>>[] includes);
        Task<TModel> GetAsync(Guid uuid, params Expression<Func<TModel, object>>[] includes);
        Guid Save(TModel model);
        void Delete(Guid uuid);
    }
}