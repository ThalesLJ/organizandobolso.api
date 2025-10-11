using Microsoft.Extensions.Logging;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Application.Services.Base;
using Microsoft.AspNetCore.Http;

namespace OrganizandoBolso.Application.Services;

public class BudgetService : BaseService<Budget>, IBudgetService
{
    public BudgetService(IBudgetRepository budgetRepository, ILogService logService, ILogger<BudgetService> logger, IHttpContextAccessor httpContextAccessor)
        : base(budgetRepository, logService, logger, httpContextAccessor)
    {
    }
}
