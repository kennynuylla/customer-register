using System;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.DataStructures;
using Services.DataStructures.Interfaces;
using Services.DataStructures.Structs;
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
                if (!await _addressRepository.CheckExistenceAsync(addressUuid)) return new NotFoundResult();
                var address = await _addressRepository.GetAsync(addressUuid);
                phone.PhoneAddressId = address.Id;
                var phoneUuid = await _localPhoneRepository.SaveAsync(phone);
                if (phoneUuid == default) return new NotFoundResult();
                return new SuccessResult<Guid>(phoneUuid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving local phone");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> DetailAsync(Guid uuid)
        {
            try
            {
                var phone = await _localPhoneRepository.GetAsync(uuid, x => x.PhoneAddress);
                if (phone is null) return new NotFoundResult();
                return new SuccessResult<LocalPhone>(phone);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving data");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> ListAsync(PaginationData pagination)
        {
            try
            {
                var phones = await _localPhoneRepository.ListAsync(pagination, x => x.PhoneAddress);
                return new SuccessResult<PaginationResult<LocalPhone>>(phones);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving data");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> DeleteAsync(Guid uuid)
        {
            try
            {
                await _localPhoneRepository.DeleteAsync(uuid);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting");
                return new FailResult();
            }
        }
    }
}