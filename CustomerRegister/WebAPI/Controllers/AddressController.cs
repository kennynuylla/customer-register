using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.Services.Interfaces;
using WebAPI.Models.Address;

namespace WebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]/{id?}")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IUnitOfWork _unitOfWork;
        
        public AddressController(IAddressService addressService, IUnitOfWork unitOfWork)
        {
            _addressService = addressService;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Gets all information from specific address
        /// </summary>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">Returns the address</response>
        /// <response code="500">An error occurred</response>     
        [HttpGet]
        public async Task<ActionResult<Address>> Get(Guid id)
        {
            throw new NotImplementedException();
            // var result = await _addressService.DetailAsync(id);
        }
        
        /// <summary>
        /// Adds an address
        /// </summary>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="500">An error occurred</response>     
        [HttpPost]
        public async Task<ActionResult> Add(SaveAddressModel model)
        {
            var address = model.GetAddress();
            var result =  _addressService.Save(address);
            if (result.IsSuccessful)
            {
                if (await _unitOfWork.SaveChangesAsync()) return new ContentResult() {StatusCode = 201};
                return new ContentResult() {StatusCode = 500};
            }

            var failure = (FailResult) result;
            return new JsonResult(failure.Errors)
            {
                StatusCode = 400
            };
        }
    }
}