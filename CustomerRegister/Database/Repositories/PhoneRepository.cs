using Database.Repositories.Base;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Database.Repositories
{
    internal class PhoneRepository : RepositoryBase<Phone>, IPhoneRepository
    {
        public PhoneRepository(ApplicationContext context, ILogger<PhoneRepository> logger) : base(context, logger)
        {
        }
    }
}