using System.Net;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.DataStructures.Interfaces;

namespace WebAPI.Controllers.Base
{
    public abstract class ApiControllerBase : Controller
    {
        protected ActionResult FailResult(IServiceResult result)
        {
            var failure = (FailResult) result;
            return new JsonResult(new {Error = failure.Errors})
            {
                StatusCode = (int) HttpStatusCode.BadRequest
            };
        }
    }
}