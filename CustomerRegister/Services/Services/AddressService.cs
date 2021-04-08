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
    internal class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger)
        {
            _addressRepository = addressRepository;
            _logger = logger;
        }

        public async Task<IServiceResult> DetailAsync(Guid uuid)
        {
            try
            {
                var address = await _addressRepository.GetAsync(uuid);
                
                if (address is null) return new FailResult(new[] {"Unable to find address"});
                return new SuccessResult<Address>(address);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving address", new {uuid});
                return new FailResult();
            }
        }

        public IServiceResult Save(Address address)
        {
            try
            {
                _addressRepository.Save(address);
                return new SuccessResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving address", address);
                return new FailResult();
            }
        }
    }
}