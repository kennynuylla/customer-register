using System;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.DataStructures;
using Services.DataStructures.Interfaces;
using Services.Repositories;
using Services.Services.Interfaces;

namespace Services.Services
{
    internal class LocalPhoneService : ILocalPhoneService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILocalPhoneRepository _localPhoneRepository;
        private readonly ILogger<LocalPhoneService> _logger;

        public LocalPhoneService(IAddressRepository addressRepository, ILocalPhoneRepository localPhoneRepository, ILogger<LocalPhoneService> logger)
        {
            _addressRepository = addressRepository;
            _localPhoneRepository = localPhoneRepository;
            _logger = logger;
        }

        public async Task<IServiceResult> SaveAsync(LocalPhone phone, Guid addressUuid)
        {
            try
            {
                var address = await _addressRepository.GetAsync(addressUuid);
                phone.PhoneAddressId = address.Id;
                var phoneUuid = _localPhoneRepository.Save(phone);
                return new SuccessResult<Guid>(phoneUuid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving local phone");
                return new FailResult();
            }
        }

        public Task<IServiceResult> GetAsync(Guid uuid)
        {
            throw new NotImplementedException();
        }
    }
}