using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using WebAPI.Controllers.Base;
using WebAPI.Models;
using WebAPI.Models.LocalPhone;

namespace WebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class LocalPhoneController : ApiControllerBase
    {

        private readonly ILocalPhoneService _localPhoneService;
        private readonly IUnitOfWork _unitOfWork;

        public LocalPhoneController(ILocalPhoneService localPhoneService, IUnitOfWork unitOfWork)
        {
            _localPhoneService = localPhoneService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a Local Phone
        /// </summary>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPost]
        [SwaggerResponseHeader(201, "Location", "string", "The new local phone URI.")]
        public async Task<ActionResult> Add(AddLocalPhoneModel model)
        {
            var (phone, addressUuid) = model.GetPhone();
            var result = await _localPhoneService.SaveAsync(phone, addressUuid);
            if (result is not SuccessResult<Guid> successResult) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return new StatusCodeResult(203);
            return ErrorResult();
        }
        
    }
}