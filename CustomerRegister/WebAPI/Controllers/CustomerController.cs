using System;
using System.Linq;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.DataStructures;
using Services.DataStructures.Structs;
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

        /// <summary>
        /// Lists the customers
        /// </summary>
        /// <param name="currentPage">Specifies the current page (starting with 1)</param>
        /// <param name="perPage">The number of entries in each page</param>
        /// <response code="200">Returns a list of customers</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpGet]
        public async Task<ActionResult<PaginationResult<CustomerListItemModel>>> List(int currentPage, int perPage)
        {
            var result = await _customerService.ListAsync(new PaginationData
            {
                CurrentPage = currentPage,
                PerPage = perPage
            });
            if (result is not SuccessResult<PaginationResult<Customer>> successResult) return FailResult(result);
            return new PaginationResult<CustomerListItemModel>
            {
                Elements = successResult.Result.Elements.Select(x => new CustomerListItemModel(x)),
                Pagination = successResult.Result.Pagination,
                Total = successResult.Result.Total
            };
        }

        /// <summary>
        /// Updates a customer
        /// </summary>
        /// <response code="204">Operation successful</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPut("{uuid}")]
        public async Task<ActionResult> Update(Guid uuid, UpdateCustomerModel model)
        {
            var customer = model.GetCustomer(uuid);
            var result = await _customerService.SaveAsync(customer);
            if (!result.IsSuccessful) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();
            return ErrorResult();
        }

        /// <summary>
        /// Deletes a customer
        /// </summary>
        /// <response code="204">Operation successful</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpDelete("{uuid}")]
        public async Task<ActionResult> Delete(Guid uuid)
        {
            var result = await _customerService.DeleteAsync(uuid);
            if (!result.IsSuccessful) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();
            return ErrorResult();
        }
    }
}