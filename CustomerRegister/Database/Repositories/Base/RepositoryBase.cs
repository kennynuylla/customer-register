using System;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Services.DataStructures.Structs;
using Services.Repositories;

namespace Database.Repositories.Base
{
    internal abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel: class, IUuidModel
    {
        protected DbSet<TModel> Set;

        public RepositoryBase(ApplicationContext context)
        {
            Set = context.Set<TModel>();
        }
        
        public Task<PaginationResult<TModel>> List(PaginationData pagination, params Func<TModel, object>[] includes)
        {
            throw new NotImplementedException();
        }

        public TModel Get(Guid uuid, params Func<TModel, object>[] includes)
        {
            throw new NotImplementedException();
        }

        public void Save(TModel model)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid uuid)
        {
            throw new NotImplementedException();
        }
    }
}