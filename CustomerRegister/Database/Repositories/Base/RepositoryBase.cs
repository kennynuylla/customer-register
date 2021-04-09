using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        
        public async Task<PaginationResult<TModel>> ListAsync(PaginationData pagination, params Expression<Func<TModel, object>>[] includes)
        {
            var query = Set
                .Skip((pagination.CurrentPage - 1) * pagination.PerPage)
                .Take(pagination.PerPage);

            query = AggregateIncludes(query, includes);
            var list = await query.ToListAsync();

            return new PaginationResult<TModel>
            {
                Elements = list,
                Pagination = pagination,
                Total = await Set.CountAsync()
            };
        }

        public async Task<TModel> GetAsync(Guid uuid, params Expression<Func<TModel, object>>[] includes)
        {
            var query = Set.Where(x => x.Uuid == uuid);
            query = AggregateIncludes(query, includes);
            return await query.FirstOrDefaultAsync();
        }

        private static IQueryable<TModel> AggregateIncludes( IQueryable<TModel> query, IEnumerable<Expression<Func<TModel, object>>> includes)
        {
            query = includes.Aggregate(query, (query, include) => query.Include(include));
            return query;
        }

        public Guid Save(TModel model)
        {
            if (model.Uuid == default)
            {
                model.Uuid = Guid.NewGuid();
                Set.Add(model);
            }
            else _context.Entry(model).State = EntityState.Modified;

            return model.Uuid;
        }

        public void Delete(Guid uuid)
        {
            throw new NotImplementedException();
        }
    }
}