using System.Net;
using Microsoft.AspNetCore.Mvc;
using Services.DataStructures;
using Services.DataStructures.Interfaces;
using NotFoundResult = Services.DataStructures.NotFoundResult;

namespace WebAPI.Controllers.Base
{
    public abstract class ApiControllerBase : Controller
    {
        protected ActionResult FailResult(IServiceResult result) => result switch
        {
            FailResult failure => new JsonResult(new {Errors = failure.Errors}) {StatusCode = (int) HttpStatusCode.BadRequest},
            NotFoundResult => NotFound(),
            _ => BadRequest()
        };
        
        protected ActionResult ErrorResult() => new StatusCodeResult(500);
    }
}