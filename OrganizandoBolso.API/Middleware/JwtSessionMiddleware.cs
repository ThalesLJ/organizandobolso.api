using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrganizandoBolso.Domain.Configuration;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrganizandoBolso.API.Middleware;

public class JwtSessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtSessionMiddleware> _logger;
    private readonly string _jwtSecret;
    private readonly ILogService _logService;

    public JwtSessionMiddleware(RequestDelegate next, ILogger<JwtSessionMiddleware> logger, IOptions<ApiSettings> apiSettings, IConfiguration configuration, ILogService logService)
    {
        _next = next;
        _logger = logger;
        _jwtSecret = configuration["JWT_SECRET"] ?? configuration["JWTSecret"] ?? string.Empty;
        _logService = logService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\n    \"message\": \"Unauthorized\",\n    \"statusCode\": 401\n}");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrWhiteSpace(_jwtSecret))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\n    \"message\": \"JWT secret not configured\",\n    \"statusCode\": 500\n}");
            return;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

            await _logService.CreateAsync(new Log { Action = "Auth", Message = "JWT validated" }, "system");

            context.Items["User"] = principal;
            var sub = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (!string.IsNullOrEmpty(sub))
            {
                context.Items["UserId"] = sub;
            }
        }
        catch (Exception ex)
        {
            await _logService.CreateAsync(new Log { Action = "AuthFailed", Message = "JWT validation failed" }, "system");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\n    \"message\": \"Unauthorized\",\n    \"statusCode\": 401\n}");
            return;
        }

        await _next(context);
    }
}
