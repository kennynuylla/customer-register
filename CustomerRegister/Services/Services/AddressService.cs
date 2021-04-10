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
    internal class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILocalPhoneRepository _localPhoneRepository;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger, ILocalPhoneRepository localPhoneRepository)
        {
            _addressRepository = addressRepository;
            _logger = logger;
            _localPhoneRepository = localPhoneRepository;
        }

        public async Task<IServiceResult> ListAsync(PaginationData pagination)
        {
            try
            {
                var paginationResult = await _addressRepository.ListAsync(pagination);
                return new SuccessResult<PaginationResult<Address>>(paginationResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while listing addresses");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> DetailAsync(Guid uuid)
        {
            try
            {
                var address = await _addressRepository.GetAsync(uuid);

                if (address is null) return new NotFoundResult(); 
                return new SuccessResult<Address>(address);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving address");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> SaveAsync(Address address)
        {
            try
            {
                var uuid = await _addressRepository.SaveAsync(address);
                if (uuid == default) return new NotFoundResult();
                return new SuccessResult<Guid>(uuid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving address");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> DeleteAsync(Guid uuid)
        {
            try
            {
                var phones = await _localPhoneRepository.GetGuidsFromAddress(uuid);
                foreach (var phoneUuid in phones)
                {
                    await _localPhoneRepository.DeleteAsync(phoneUuid);
                }
                await _addressRepository.DeleteAsync(uuid);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting address");
                return new FailResult();
            }
        }
    }
}