using System;
using System.Threading.Tasks;
using Domain.Models;
using Services.DataStructures.Interfaces;
using Services.Services.Interfaces;

namespace Services.Services
{
    internal class LocalPhoneService : ILocalPhoneService
    {
        public IServiceResult SaveAsync(LocalPhone phone, Guid addressUuid)
        {
            throw new NotImplementedException();
        }

        public Task<IServiceResult> GetAsync(Guid uuid)
        {
            throw new NotImplementedException();
        }
    }
}