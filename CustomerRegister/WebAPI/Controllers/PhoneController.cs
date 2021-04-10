using System;
using System.Linq;
using System.Threading.Tasks;
using Database.UnitOfWork.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.DataStructures.Structs;
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
            if (await _unitOfWork.SaveChangesAsync()) return CreatedAtAction(nameof(Get), new {uuid = successResult.Result}, null);
            return ErrorResult();
        }

        /// <summary>
        /// Gets all information from specific phone
        /// </summary>
        /// <returns>An existing phone</returns>
        /// <response code="200">Returns the  phone</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Phone not found</response>
        /// <response code="500">An error occurred</response>     
        [HttpGet("{uuid}")]
        public async Task<ActionResult<Phone>> Get(Guid uuid)
        {
            var result = await _phoneService.DetailAsync(uuid);
            if (result is SuccessResult<Phone> successResult) return successResult.Result;
            return FailResult(result);
        }

        /// <summary>
        /// Lists the phones
        /// </summary>
        /// <param name="currentPage">Specifies the current page (starting with 1)</param>
        /// <param name="perPage">The number of entries in each page</param>
        /// <response code="200">Returns a list of phones</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpGet]
        public async Task<ActionResult<PaginationResult<PhoneListItemModel>>> List(int currentPage, int perPage)
        {
            var result = await _phoneService.ListAsync(new PaginationData
            {
                CurrentPage = currentPage,
                PerPage = perPage
            });
            if (result is not SuccessResult<PaginationResult<Phone>> successResult) return FailResult(result);
            return new PaginationResult<PhoneListItemModel>
            {
                Elements = successResult.Result.Elements.Select(x => new PhoneListItemModel(x)),
                Pagination = successResult.Result.Pagination,
                Total = successResult.Result.Total
            };
        }
    }
}