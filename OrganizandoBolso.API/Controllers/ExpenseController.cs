using Microsoft.AspNetCore.Mvc;
using OrganizandoBolso.API.Controllers.Base;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Domain.Models.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace OrganizandoBolso.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Expenses management")]
public class ExpenseController : BaseController
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List all expenses")]
    [SwaggerResponse(200, "Expenses list returned successfully")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<Expense>>>> GetAll()
    {
        var response = await _expenseService.GetAllAsync();
        return HandleServiceResponse(response);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get an expense by ID")]
    [SwaggerResponse(200, "Expense found successfully")]
    [SwaggerResponse(404, "Expense not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<Expense>>> GetById(string id)
    {
        var response = await _expenseService.GetByIdAsync(id);
        return HandleServiceResponse(response);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new expense")]
    [SwaggerResponse(201, "Expense created successfully")]
    [SwaggerResponse(400, "Invalid data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<Expense>>> Create([FromBody] Expense expense)
    {
        var user = GetCurrentUser();
        var response = await _expenseService.CreateAsync(expense, user);
        return HandleServiceResponse(response);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing expense")]
    [SwaggerResponse(200, "Expense updated successfully")]
    [SwaggerResponse(400, "Invalid data")]
    [SwaggerResponse(404, "Expense not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<Expense>>> Update(string id, [FromBody] Expense expense)
    {
        if (id != expense.Id)
        {
            return BadRequestResponse<Expense>("URL ID does not match expense ID");
        }

        var user = GetCurrentUser();
        var response = await _expenseService.UpdateAsync(expense, user);
        return HandleServiceResponse(response);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an expense")]
    [SwaggerResponse(200, "Expense deleted successfully")]
    [SwaggerResponse(404, "Expense not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<ServiceResponse<bool>>> Delete(string id)
    {
        var user = GetCurrentUser();
        var response = await _expenseService.DeleteAsync(id, user);
        return HandleServiceResponse(response);
    }
}
