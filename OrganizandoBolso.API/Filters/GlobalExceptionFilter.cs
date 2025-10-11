using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Domain.Models.Base;
using System.Net;

namespace OrganizandoBolso.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly ILogService _logService;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, ILogService logService)
    {
        _logger = logger;
        _logService = logService;
    }

    public void OnException(ExceptionContext context)
    {
        var message = context.Exception?.Message ?? "Unhandled exception";
        _logService.CreateAsync(new Log { Action = "UnhandledException", Message = message }, "system");

        var response = ServiceResponse<object>.ErrorResponse(
            "An internal server error occurred. Please try again later.",
            (int)HttpStatusCode.InternalServerError,
            new List<string> { context.Exception.Message }
        );

        context.Result = new ObjectResult(response)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
