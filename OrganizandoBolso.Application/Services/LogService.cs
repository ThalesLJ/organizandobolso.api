using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Application.Services.Base;

namespace OrganizandoBolso.Application.Services;

public class LogService : BaseService<Log>, ILogService
{
    public LogService(ILogRepository logRepository, ILogger<LogService> logger, IHttpContextAccessor httpContextAccessor)
        : base(logRepository, null!, logger, httpContextAccessor)
    {
    }
}
