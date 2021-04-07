using Database.Repositories.Base;
using Domain.Models;
using Services.Repositories;

namespace Database.Repositories
{
    internal class AddressRepository : RepositoryBase<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationContext context) : base(context)
        {
        }
    }
}