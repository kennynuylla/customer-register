using Database.Repositories.Base;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Database.Repositories
{
    internal class AddressRepository : RepositoryBase<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationContext context, ILogger<AddressRepository> logger) : base(context, logger)
        {
        }
    }
}