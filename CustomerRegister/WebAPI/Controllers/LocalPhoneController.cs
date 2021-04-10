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

        /// <summary>
        /// Lists the local phones
        /// </summary>
        /// <param name="currentPage">Specifies the current page (starting with 1)</param>
        /// <param name="perPage">The number of entries in each page</param>
        /// <response code="200">Returns a list of local phones</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpGet]
        public async Task<ActionResult<PaginationResult<LocalPhoneListItemModel>>> List(int currentPage, int perPage)
        {
            var result = await _localPhoneService.ListAsync(new PaginationData
            {
                CurrentPage = currentPage,
                PerPage = perPage
            });
            if (result is not SuccessResult<PaginationResult<LocalPhone>> successResult) return FailResult(result);
            var listItems = successResult.Result.Elements.Select(x => new LocalPhoneListItemModel(x));

            return new PaginationResult<LocalPhoneListItemModel>
            {
                Elements = listItems,
                Pagination = successResult.Result.Pagination,
                Total = successResult.Result.Total
            };
        }

        /// <summary>
        /// Updates a local phone
        /// </summary>
        /// <response code="204">Operation successful</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpPut("{uuid}")]
        public async Task<ActionResult> Update(Guid uuid, UpdateLocalPhoneModel model)
        {
            var (phone, addressUuid) = model.GetPhone(uuid);
            var result = await _localPhoneService.SaveAsync(phone, addressUuid);
            if (!result.IsSuccessful) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();
            return ErrorResult();
        }

        /// <summary>
        /// Deletes a local phone
        /// </summary>
        /// <response code="204">Operation successful</response>
        /// <response code="400" >Bad Request</response>
        /// <response code="500">An error occurred</response>
        [HttpDelete("{uuid}")]
        public async Task<ActionResult> Delete(Guid uuid)
        {
            var result = await _localPhoneService.DeleteAsync(uuid);
            if (!result.IsSuccessful) return FailResult(result);
            if (await _unitOfWork.SaveChangesAsync()) return NoContent();
            return ErrorResult();
        }
        
    }
}