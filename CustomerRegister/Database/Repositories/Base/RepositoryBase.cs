using System;
using System.Threading.Tasks;
using Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Services.DataStructures.Structs;
using Services.Repositories;

namespace Database.Repositories.Base
{
    internal abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel: class, IUuidModel
    {
        protected ApplicationContext _context;
        protected DbSet<TModel> Set;

        public RepositoryBase(ApplicationContext context)
        {
            _context = context;
            Set = context.Set<TModel>();
        }
        
        public Task<PaginationResult<TModel>> List(PaginationData pagination, params Func<TModel, object>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> Get(Guid uuid, params Func<TModel, object>[] includes)
        {
            throw new NotImplementedException();
        }

        public void Save(TModel model)
        {
            if (model.Uuid == default)
            {
                model.Uuid = Guid.NewGuid();
                Set.Add(model);
            }
            else _context.Entry(model).State = EntityState.Modified;
        }

        public void Delete(Guid uuid)
        {
            throw new NotImplementedException();
        }
    }
}