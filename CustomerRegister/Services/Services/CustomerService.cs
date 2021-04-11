using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    internal class CustomerService : ICustomerService
    {
        private  readonly ICustomerRepository _customerRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IPhoneRepository _phoneRepository;
        private readonly ILocalPhoneRepository _localPhoneRepository;
        private  readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger, IPhoneRepository phoneRepository, IAddressRepository addressRepository, ILocalPhoneRepository localPhoneRepository)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _phoneRepository = phoneRepository;
            _addressRepository = addressRepository;
            _localPhoneRepository = localPhoneRepository;
        }

        public async Task<IServiceResult> SaveAsync(Customer customer, IEnumerable<Guid> addresses)
        {
            try
            {
                if (await _customerRepository.CheckEmailAlreadyRegisteredAsync(customer.Email))
                    return new FailResult(new[]
                    {
                        "Email already registered."
                    });
                var insertedUuid = await GetInsertedUuidAsync(customer, addresses);
                return insertedUuid == default ? (IServiceResult) new NotFoundResult() : new SuccessResult<Guid>(insertedUuid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while saving");
                return new FailResult();
            }
        }

        private async Task<Guid> GetInsertedUuidAsync(Customer customer, IEnumerable<Guid> addresses)
        {
            var addressesToAdd =  new List<Address>();
            foreach (var uuid in addresses)
            {
                var address = await _addressRepository.GetTrackedAsync(uuid);
                addressesToAdd.Add(address);
            }
            customer.Addresses = addressesToAdd;
            var insertedUuid = await _customerRepository.SaveAsync(customer);
            return insertedUuid;
        }
        


        public async Task<IServiceResult> DetailAsync(Guid uuid)
        {
            try
            {
                var address = await _addressRepository.GetAddressesFromCustomer(uuid);
                var localPhones = await GetPhonesAsync(address);
                var phones = await _phoneRepository.GetPhonesFromCustomerAsync(uuid);
                var customer = await _customerRepository.GetAsync(uuid, x=> x.Addresses);
                if (customer is null) return new NotFoundResult();
                var result = new CustomerContainer
                {
                    Customer = customer,
                    Addresses = address,
                    LocalPhones = localPhones,
                    Phones = phones
                };
                return new SuccessResult<CustomerContainer>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving data");
                return new FailResult();
            }
        }

        private async Task<IEnumerable<LocalPhone>> GetPhonesAsync(IEnumerable<Address> addresses)
        {
            var phones = new List<LocalPhone>();
            foreach (var address in addresses)
            {
                var phone = await _localPhoneRepository.GetFromAddressAsync(address.Uuid);
                phones.AddRange(phone);
            }

            return phones;
        }

        public async Task<IServiceResult> ListAsync(PaginationData pagination)
        {
            try
            {
                var customers = await _customerRepository.ListAsync(pagination);
                return new SuccessResult<PaginationResult<Customer>>(customers);
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
                var phonesUuids = await _phoneRepository.GetUuidsFromCustomer(uuid);
                foreach (var phoneUuid in phonesUuids)
                {
                    await _phoneRepository.DeleteAsync(phoneUuid);
                }
                await _customerRepository.DeleteAsync(uuid);
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