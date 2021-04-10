using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.DataStructures.Structs;
using Services.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using WebAPI.Controllers.Base;
using WebAPI.Models.Address;

namespace WebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class AddressController : ApiControllerBase
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
        /// <returns>An existing address</returns>
        /// <response code="200">Returns the address</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Address not found</response>
        /// <response code="500">An error occurred</response>     
        [HttpGet("{uuid}")]
        public async Task<ActionResult<Address>> Get(Guid uuid)
        {
            var result = await _addressService.DetailAsync(uuid);
            if (result is not SuccessResult<Address> successResult) return FailResult(result);
            return successResult.Result;
        }
        
        /// <summary>
        /// Adds an address
        /// </summary>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPost]
        [SwaggerResponseHeader(201, "Location", "string", "The new address URI.")]
        public async Task<ActionResult> Add(AddAddressModel model)
        {
            var address = model.GetAddress();
            var result =  _addressService.Save(address);
            
            if (result is not SuccessResult<Guid> successResult) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return CreatedAtAction(nameof(Get), new {uuid = successResult.Result}, null);
            return ErrorResult();
        }

        /// <summary>
        /// Lists the addresses
        /// </summary>
        /// <param name="currentPage">Specifies the current page (starting with 1)</param>
        /// <param name="perPage">The number of entries in each page</param>
        /// <response code="200">Returns a list of addresses</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpGet]
        public async Task<ActionResult<PaginationResult<AddressListItem>>> List(int currentPage, int perPage)
        {
            var result = await _addressService.ListAsync(new PaginationData
            {
                CurrentPage = currentPage,
                PerPage = perPage
            });
            if (result is not SuccessResult<PaginationResult<Address>> successResult) return FailResult(result);

            return new PaginationResult<AddressListItem>
            {
                Elements = successResult.Result.Elements.Select(x => new AddressListItem(x)),
                Pagination = successResult.Result.Pagination,
                Total = successResult.Result.Total
            };
        }
        
        /// <summary>
        /// Updates an address
        /// </summary>
        /// <response code="204">Operation successful</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPut("{uuid}")]
        public async Task<ActionResult> Update(Guid uuid, UpdateAddressModel model)
        {
            var address = model.GetAddress(uuid);
            var result =  _addressService.Save(address);

            if (!result.IsSuccessful) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();
            return ErrorResult();
        }

        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <response code="204">Operation successful</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpDelete("{uuid}")]
        public async Task<ActionResult> Delete(Guid uuid)
        {
            var result = await _addressService.DeleteAsync(uuid);
            if (!result.IsSuccessful) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();
            return ErrorResult();
        }
    }
}