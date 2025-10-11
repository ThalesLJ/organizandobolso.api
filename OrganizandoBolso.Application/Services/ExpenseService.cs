using Microsoft.Extensions.Logging;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Application.Services.Base;
using Microsoft.AspNetCore.Http;

namespace OrganizandoBolso.Application.Services;

public class ExpenseService : BaseService<Expense>, IExpenseService
{
    public ExpenseService(IExpenseRepository expenseRepository, ILogService logService, ILogger<ExpenseService> logger, IHttpContextAccessor httpContextAccessor)
        : base(expenseRepository, logService, logger, httpContextAccessor)
    {
    }
}
