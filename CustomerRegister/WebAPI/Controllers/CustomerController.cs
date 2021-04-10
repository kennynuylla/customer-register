using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.DataStructures;
using Services.Services.Interfaces;
using WebAPI.Controllers.Base;
using WebAPI.Models.Customer;

namespace WebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class CustomerController : ApiControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(ICustomerService customerService, IUnitOfWork unitOfWork)
        {
            _customerService = customerService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a customer
        /// </summary>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPost]
        public async Task<ActionResult> Add(AddCustomerModel model)
        {
            var customer = model.GetCustomer();
            var result = await _customerService.SaveAsync(customer);
            if (result is not SuccessResult<Guid> successResult) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return CreatedAtAction(nameof(Get), new {uuid = successResult.Result}, null);
            return ErrorResult();
        }

        /// <summary>
        /// Gets all information from specific customer
        /// </summary>
        /// <returns>An existing customer</returns>
        /// <response code="200">Returns the customer</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">customer not found</response>
        /// <response code="500">An error occurred</response>     
        [HttpGet("{uuid}")]
        public async Task<ActionResult<Customer>> Get(Guid uuid)
        {
            var result = await _customerService.DetailAsync(uuid);
            if (result is not SuccessResult<Customer> successResult) return FailResult(result);
            return successResult.Result;
        }
    }
}