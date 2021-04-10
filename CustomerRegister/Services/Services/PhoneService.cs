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
    internal class PhoneService : IPhoneService
    {
        private readonly IPhoneRepository _phoneRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<PhoneService> _logger;

        public PhoneService(IPhoneRepository phoneRepository, ILogger<PhoneService> logger, ICustomerRepository customerRepository)
        {
            _phoneRepository = phoneRepository;
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<IServiceResult> SaveAsync(Phone phone, Guid customerUuid)
        {
            try
            {
                if (!await _customerRepository.CheckExistenceAsync(customerUuid)) return new NotFoundResult();
                var customer = await _customerRepository.GetAsync(customerUuid);
                phone.CustomerId = customer.Id;
                var uuid = await _phoneRepository.SaveAsync(phone);
                return uuid == default ? new NotFoundResult() : new SuccessResult<Guid>(uuid);
            }
            catch (Exception e)
            {
               _logger.LogError(e, "Error while saving");
               return new FailResult();
            }
        }
    }
}