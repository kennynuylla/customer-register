﻿using System;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Services.DataStructures;
using Services.DataStructures.Interfaces;
using Services.Repositories;
using Services.Services.Interfaces;

namespace Services.Services
{
    internal class CustomerService : ICustomerService
    {
        private  readonly ICustomerRepository _customerRepository;
        private  readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IServiceResult> SaveAsync(Customer customer)
        {
            try
            {
                var insertedUuid = await _customerRepository.SaveAsync(customer);
                return insertedUuid == default ? (IServiceResult) new NotFoundResult() : new SuccessResult<Guid>(insertedUuid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving");
                return new FailResult();
            }
        }

        public async Task<IServiceResult> DetailAsync(Guid uuid)
        {
            try
            {
                var customer = await _customerRepository.GetAsync(uuid);
                return customer is null ? new NotFoundResult() : new SuccessResult<Customer>(customer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving data");
                return new FailResult();
            }
        }
    }
}