using EKO.PingPingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EKO.PingPingApi.Filters;

/// <summary>
/// Filter for handling ArgumentNullExceptions.
/// </summary>
public sealed class ArgumentNullExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ArgumentNullException)
        {
            context.Result = new BadRequestObjectResult(new ErrorModel { Message = context.Exception.Message });
            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
