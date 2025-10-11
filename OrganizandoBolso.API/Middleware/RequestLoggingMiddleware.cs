using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;

namespace OrganizandoBolso.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly ILogService _logService;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, ILogService logService)
    {
        _next = next;
        _logger = logger;
        _logService = logService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        var isSearch = string.Equals(requestMethod, "GET", StringComparison.OrdinalIgnoreCase);
        if (!isSearch)
        {
            await _logService.CreateAsync(new Log { Action = "RequestStarted", Message = $"{requestMethod} {requestPath}" }, "system");
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var isSearchError = string.Equals(requestMethod, "GET", StringComparison.OrdinalIgnoreCase);
            if (!isSearchError)
            {
                await _logService.CreateAsync(new Log { Action = "RequestError", Message = $"{requestMethod} {requestPath} - {ex.Message}" }, "system");
            }
            throw;
        }
        finally
        {
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            var statusCode = context.Response.StatusCode;

            if (!isSearch)
            {
                await _logService.CreateAsync(new Log { Action = "RequestFinished", Message = $"{requestMethod} {requestPath} - {statusCode} - {duration.TotalMilliseconds}ms" }, "system");
            }
        }
    }
}
