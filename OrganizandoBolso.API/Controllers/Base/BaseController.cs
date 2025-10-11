using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrganizandoBolso.API.Filters;
using OrganizandoBolso.Domain.Models.Base;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace OrganizandoBolso.API.Controllers.Base;

[ApiController]
[RequireBearer]
[Route("api/[controller]")]
[SwaggerTag("Base controller with common operations")]
public abstract class BaseController : ControllerBase
{
    protected string GetCurrentUser()
    {
        var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return user ?? "anonymous";
    }

    protected string GetCurrentUserEmail()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        return email ?? "anonymous@example.com";
    }

    protected ActionResult<ServiceResponse<T>> HandleServiceResponse<T>(ServiceResponse<T> response)
    {
        if (response.Success)
        {
            return response.StatusCode switch
            {
                200 => Ok(response),
                201 => CreatedAtAction(nameof(Get), new { id = GetEntityId(response.Data) }, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }

        return response.StatusCode switch
        {
            400 => BadRequest(response),
            401 => Unauthorized(response),
            403 => Forbid(),
            404 => NotFound(response),
            409 => Conflict(response),
            _ => StatusCode(response.StatusCode, response)
        };
    }

    protected virtual string? GetEntityId<T>(T? entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            return baseEntity.Id;
        }
        return null;
    }

    protected ActionResult<ServiceResponse<T>> CreatedResponse<T>(T data, string message = "Entity created successfully")
    {
        var response = ServiceResponse<T>.SuccessResponse(data, message);
        response.StatusCode = 201;
        return CreatedAtAction(nameof(Get), new { id = GetEntityId(data) }, response);
    }

    protected ActionResult<ServiceResponse<T>> UpdatedResponse<T>(T data, string message = "Entity updated successfully")
    {
        var response = ServiceResponse<T>.SuccessResponse(data, message);
        return Ok(response);
    }

    protected ActionResult<ServiceResponse<bool>> DeletedResponse(string message = "Entity deleted successfully")
    {
        var response = ServiceResponse<bool>.SuccessResponse(true, message);
        return Ok(response);
    }

    protected ActionResult<ServiceResponse<T>> NotFoundResponse<T>(string message = "Entity not found")
    {
        var response = ServiceResponse<T>.ErrorResponse(message, 404);
        return NotFound(response);
    }

    protected ActionResult<ServiceResponse<T>> BadRequestResponse<T>(string message, List<string>? errors = null)
    {
        var response = ServiceResponse<T>.ErrorResponse(message, 400, errors);
        return BadRequest(response);
    }

    protected ActionResult<ServiceResponse<T>> InternalServerErrorResponse<T>(string message = "Internal server error")
    {
        var response = ServiceResponse<T>.ErrorResponse(message, 500);
        return StatusCode(500, response);
    }

    protected virtual IActionResult Get()
    {
        return NotFound();
    }
}
