using CwkSocial.Api.Contracts.Common;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

public class BaseController : ControllerBase
{
    protected IActionResult HandleErrorResponse(List<Error> errors)
    {
        if (errors.Any(e => e.Code == ErrorCode.NotFound))
        {
            var errorMessage = errors.First(e => e.Code == ErrorCode.NotFound).Message;

            var apiError = new ErrorResponse
            {
                StatusCode = 404,
                StatusPhrase = "Not Found",
                TimeStamp = DateTime.Now,
                Errors = { errorMessage }
            };
            return NotFound(apiError);
        }
            
        var apiError2 = new ErrorResponse
        {
            StatusCode = 500,
            StatusPhrase = "Internal Server Error",
            TimeStamp = DateTime.Now,
            Errors = { "Unknown error" }
        };
        return StatusCode(500, apiError2);
    }
}