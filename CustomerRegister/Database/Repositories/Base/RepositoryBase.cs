using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.DataStructures.Structs;
using Services.Repositories;

namespace Database.Repositories.Base
{
    internal abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel : class, IBaseModel
    {
        protected readonly ApplicationContext Context;
        protected DbSet<TModel> Set;
        protected ILogger<RepositoryBase<TModel>> Logger;

        public RepositoryBase(ApplicationContext context, ILogger<RepositoryBase<TModel>> logger)
        {
            Context = context;
            Logger = logger;
            Set = context.Set<TModel>();
        }

        public async Task<PaginationResult<TModel>> ListAsync(PaginationData pagination, params Expression<Func<TModel, object>>[] includes)
        {
            var filteredQuery = Set
                .AsNoTracking()
                .Where(x => x.IsActive);

            var query = AggregateIncludes(filteredQuery, includes)
                .Skip((pagination.CurrentPage - 1) * pagination.PerPage)
                .Take(pagination.PerPage)
                .OrderBy(x => x.Id);
            
            var list = await query.ToListAsync();

            return new PaginationResult<TModel>
            {
                Elements = list,
                Pagination = pagination,
                Total = await filteredQuery.CountAsync()
            };
        }

        public async Task<TModel> GetAsync(Guid uuid, params Expression<Func<TModel, object>>[] includes)
        {
            var query = Set
                .AsNoTracking()
                .Where(x => x.Uuid == uuid && x.IsActive);
            
            query = AggregateIncludes(query, includes);
            return await query.FirstOrDefaultAsync();
        }

        private static IQueryable<TModel> AggregateIncludes(IQueryable<TModel> query, IEnumerable<Expression<Func<TModel, object>>> includes)
        {
            query = includes.Aggregate(query, (query, include) => query.Include(include));
            return query;
        }

        public async Task<Guid> SaveAsync(TModel model)
        {
            if (model.Uuid == default)
            {
                model.Uuid = Guid.NewGuid();
                await Set.AddAsync(model);
            }
            else
            {
                if(!await CheckExistenceAsync(model.Uuid)) return Guid.Empty;
                Context.Entry(model).State = EntityState.Modified;
            }

            return model.Uuid;
        }

        public async Task DeleteAsync(Guid uuid)
        {
            var addressToDeleteTracked = await Set.FirstOrDefaultAsync(x => x.Uuid == uuid && x.IsActive);
            if(addressToDeleteTracked is not null) addressToDeleteTracked.IsActive = false;
            else Logger.LogWarning("Repository: You are trying to delete a non existing model of type {0} with Uuid={1}. Take care.", typeof(TModel), uuid);
        }

        public async Task<bool> CheckExistenceAsync(Guid uuid)
        {
            var total = await Set.Where(x => x.Uuid == uuid && x.IsActive).CountAsync();
            return total == 1;
        }
    }
}