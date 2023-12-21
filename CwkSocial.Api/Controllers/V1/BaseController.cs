using CwkSocial.Api.Contracts.Common;
using CwkSocial.Application.Enums;
using CwkSocial.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CwkSocial.Api.Controllers.V1;

public class BaseController : ControllerBase
{
    protected IActionResult HandleErrorResponse(List<Error> errors)
    {
        var apiError = new ErrorResponse();
        if (errors.Any(e => e.Code == ErrorCode.NotFound))
        {
            var errorMessage = errors.First(e => e.Code == ErrorCode.NotFound).Message;

            apiError.StatusCode = 404;
            apiError.StatusPhrase = "Not Found";
            apiError.TimeStamp = DateTime.Now;
            apiError.Errors.Add(errorMessage);
            
            return NotFound(apiError);
        }

        if (errors.Any(e => e.Code == ErrorCode.ValidationError))
        {
            apiError.StatusCode = 400;
            apiError.StatusPhrase = "Bad Request";
            apiError.TimeStamp = DateTime.Now;
            errors.ForEach(e => apiError.Errors.Add(e.Message));
            return StatusCode(400, apiError);
        }
        apiError.StatusCode = 500;
        apiError.StatusPhrase = "Internal Server Error";
        apiError.TimeStamp = DateTime.Now;
        apiError.Errors.Add("Unknown error");
        return StatusCode(500, apiError);
    }
}