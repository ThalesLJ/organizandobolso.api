using Microsoft.AspNetCore.Mvc.Filters;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using System.Text.Json;

namespace OrganizandoBolso.API.Filters;

public class AuditFilter : IActionFilter
{
    private readonly ILogger<AuditFilter> _logger;
    private readonly ILogService _logService;

    public AuditFilter(ILogger<AuditFilter> logger, ILogService logService)
    {
        _logger = logger;
        _logService = logService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var controllerName = context.Controller.GetType().Name;
        var httpMethod = context.HttpContext.Request.Method;
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        var isSearch = string.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase);
        if (!isSearch)
        {
            string? body = null;
            try
            {
                context.HttpContext.Request.EnableBuffering();
                using var reader = new StreamReader(context.HttpContext.Request.Body, leaveOpen: true);
                body = reader.ReadToEndAsync().GetAwaiter().GetResult();
                context.HttpContext.Request.Body.Position = 0;
            }
            catch { }

            var message = string.IsNullOrWhiteSpace(body)
                ? $"{controllerName}.{actionName} {httpMethod}"
                : $"{controllerName}.{actionName} {httpMethod} | Body: {body}";

            _ = _logService.CreateAsync(new Log { Action = "ActionExecuting", Message = message }, "system");
        }

        context.HttpContext.Items["AuditInfo"] = new
        {
            ActionName = actionName,
            ControllerName = controllerName,
            HttpMethod = httpMethod,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow
        };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var auditInfo = context.HttpContext.Items["AuditInfo"];
        var statusCode = context.HttpContext.Response.StatusCode;
        var isSuccess = statusCode >= 200 && statusCode < 300;

        if (auditInfo != null)
        {
            var audit = JsonSerializer.Serialize(auditInfo);
            var controllerName = context.Controller.GetType().Name;
            var actionName = context.ActionDescriptor.DisplayName;
            var httpMethod = context.HttpContext.Request.Method;
            var isSearch = string.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase);
            if (!isSearch)
            {
                _ = _logService.CreateAsync(new Log { Action = "ActionExecuted", Message = $"{controllerName}.{actionName} {statusCode} {isSuccess}" }, "system");
            }
        }
    }
}
