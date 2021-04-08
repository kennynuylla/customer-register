using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using WebAPI.Models.Address;

namespace WebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]")]
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
        /// Adds an address
        /// </summary>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="500">An error occurred</response>     
        [HttpPost]
        public async Task<ActionResult> Add(SaveAddressModel model)
        {
            throw new NotImplementedException();
        }
    }
}