using CwkSocial.Api.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CwkSocial.Api.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var apiError = new ErrorResponse
            {
                StatusCode = 400,
                StatusPhrase = "Bad Request",
                TimeStamp = DateTime.Now
            };
            var errors = context.ModelState.AsEnumerable();

            foreach (var error in errors)
            {
                apiError.Errors.Add(error.Value.ToString());
            }

            context.Result = new JsonResult(apiError) { StatusCode = 400 };
        }
    }
}