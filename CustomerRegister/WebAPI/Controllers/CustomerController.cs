using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
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
            if (await _unitOfWork.SaveChangesAsync()) return new StatusCodeResult(201);
            return ErrorResult();
        }
    }
}