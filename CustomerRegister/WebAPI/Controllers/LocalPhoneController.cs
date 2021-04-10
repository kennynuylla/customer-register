using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
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
            if (await _unitOfWork.SaveChangesAsync()) return CreatedAtAction(nameof(Get), new{uuid = successResult.Result}, null);
            return ErrorResult();
        }

        /// <summary>
        /// Gets all information from specific local phone
        /// </summary>
        /// <returns>An existing local phone</returns>
        /// <response code="200">Returns the local phone</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Local phone not found</response>
        /// <response code="500">An error occurred</response>     
        [HttpGet("{uuid}")]
        public async Task<ActionResult<LocalPhone>> Get(Guid uuid)
        {
            var result = await _localPhoneService.DetailAsync(uuid);
            if (result is SuccessResult<LocalPhone> successResult) return successResult.Result;
            return FailResult(result);
        }
        
    }
}