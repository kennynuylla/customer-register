using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Microsoft.Extensions.Logging;

namespace Database.UnitOfWork
{
    internal class UnitOfWork : IUnitOfWork
    {
        private ApplicationContext _context;
        private ILogger<UnitOfWork> _logger;

        public UnitOfWork(ApplicationContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving");
                return false;
            }
        }
    }
}