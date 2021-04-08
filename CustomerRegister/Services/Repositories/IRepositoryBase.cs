using System;
using System.Threading.Tasks;
using Domain.Models.Interfaces;
using Services.DataStructures.Structs;

namespace Services.Repositories
{
    public interface IRepositoryBase<TModel> where TModel:IUuidModel
    {
        Task<PaginationResult<TModel>> List(PaginationData pagination, params Func<TModel, object>[] includes);
        Task<TModel> Get(Guid uuid, params Func<TModel, object>[] includes);
        void Save(TModel model);
        void Delete(Guid uuid);
    }
}