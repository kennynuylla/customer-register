using Database.Repositories.Base;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Database.Repositories
{
    internal class LocalPhoneRepository : RepositoryBase<LocalPhone>, ILocalPhoneRepository
    {
        public LocalPhoneRepository(ApplicationContext context, ILogger<LocalPhoneRepository> logger) : base(context, logger)
        {
        }
    }
}