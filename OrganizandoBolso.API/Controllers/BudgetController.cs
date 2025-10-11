using Microsoft.AspNetCore.Mvc;
using OrganizandoBolso.API.Controllers.Base;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Domain.Models.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace OrganizandoBolso.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Budgets management")]
public class BudgetController : BaseController
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List all budgets")]
    [SwaggerResponse(200, "Budgets list returned successfully")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<Budget>>>> GetAll()
    {
        var response = await _budgetService.GetAllAsync();
        return HandleServiceResponse(response);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a budget by ID")]
    [SwaggerResponse(200, "Budget found successfully")]
    [SwaggerResponse(404, "Budget not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<Budget>>> GetById(string id)
    {
        var response = await _budgetService.GetByIdAsync(id);
        return HandleServiceResponse(response);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new budget")]
    [SwaggerResponse(201, "Budget created successfully")]
    [SwaggerResponse(400, "Invalid data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<Budget>>> Create([FromBody] Budget budget)
    {
        var user = GetCurrentUser();
        var response = await _budgetService.CreateAsync(budget, user);
        return HandleServiceResponse(response);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing budget")]
    [SwaggerResponse(200, "Budget updated successfully")]
    [SwaggerResponse(400, "Invalid data")]
    [SwaggerResponse(404, "Budget not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<Budget>>> Update(string id, [FromBody] Budget budget)
    {
        if (id != budget.Id)
        {
            return BadRequestResponse<Budget>("URL ID does not match budget ID");
        }

        var user = GetCurrentUser();
        var response = await _budgetService.UpdateAsync(budget, user);
        return HandleServiceResponse(response);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a budget")]
    [SwaggerResponse(200, "Budget deleted successfully")]
    [SwaggerResponse(404, "Budget not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<bool>>> Delete(string id)
    {
        var user = GetCurrentUser();
        var response = await _budgetService.DeleteAsync(id, user);
        return HandleServiceResponse(response);
    }
}
