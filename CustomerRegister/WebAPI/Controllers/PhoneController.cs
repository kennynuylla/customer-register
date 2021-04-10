using System;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using WebAPI.Controllers.Base;
using WebAPI.Models.LocalPhone;
using WebAPI.Models.Phone;

namespace WebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class PhoneController : ApiControllerBase
    {

        private readonly IPhoneService _phoneService;
        private readonly IUnitOfWork _unitOfWork;

        public PhoneController(IPhoneService phoneService, IUnitOfWork unitOfWork)
        {
            _phoneService = phoneService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a Phone
        /// </summary>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPost]
        [SwaggerResponseHeader(201, "Location", "string", "The new phone URI.")]
        public async Task<ActionResult> Add(AddPhoneModel model)
        {
            var (phone, customerUuid) = model.GetPhone();
            var result = await _phoneService.SaveAsync(phone, customerUuid);
            if (result is not SuccessResult<Guid> successResult) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return new StatusCodeResult(201);
            return ErrorResult();
        }
    }
}