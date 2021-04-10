﻿using System.Net;
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
            FailResult failure => new JsonResult(new {Error = failure.Errors}) {StatusCode = (int) HttpStatusCode.BadRequest},
            NotFoundResult => NotFound(),
            _ => BadRequest()
        };
    }
}