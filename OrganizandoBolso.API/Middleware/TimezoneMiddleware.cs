namespace OrganizandoBolso.API.Middleware;

public class TimezoneMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TimezoneMiddleware> _logger;

    public TimezoneMiddleware(RequestDelegate next, ILogger<TimezoneMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            TimeZoneInfo.ClearCachedData();
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            
            if (timezone != null)
            {
                _logger.LogDebug("Timezone set to: {Timezone}", timezone.DisplayName);
            }
            else
            {
                _logger.LogWarning("Timezone America/Sao_Paulo not found, using system default");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error configuring timezone, using system default");
        }

        await _next(context);
    }
}
