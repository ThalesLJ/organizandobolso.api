using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Application.Services.Base;

public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
{
    protected readonly IBaseRepository<T> Repository;
    protected readonly ILogService? LogService;
    protected readonly ILogger<BaseService<T>> Logger;
    protected readonly IHttpContextAccessor HttpContextAccessor;

    protected BaseService(IBaseRepository<T> repository, ILogService? logService, ILogger<BaseService<T>> logger, IHttpContextAccessor httpContextAccessor)
    {
        Repository = repository;
        LogService = logService;
        Logger = logger;
        HttpContextAccessor = httpContextAccessor;
    }

    protected string GetCurrentUserId()
    {
        var context = HttpContextAccessor.HttpContext;
        if (context?.Items["UserId"] is string sub && !string.IsNullOrEmpty(sub))
        {
            return sub;
        }
        var claim = context?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? context?.User?.FindFirst("sub")?.Value;
        return claim ?? string.Empty;
    }

    public virtual async Task<ServiceResponse<T>> GetByIdAsync(string id)
    {
        try
        {
            Logger.LogInformation("Fetching entity {EntityType} with ID: {Id}", typeof(T).Name, id);

            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                Logger.LogWarning("Entity {EntityType} with ID {Id} not found", typeof(T).Name, id);
                return ServiceResponse<T>.ErrorResponse($"Entity with ID {id} not found", 404);
            }

            var userId = GetCurrentUserId();
            var userProp = typeof(T).GetProperty("UserId");
            if (userProp != null && userProp.GetValue(entity)?.ToString() != userId)
            {
                return ServiceResponse<T>.ErrorResponse($"Entity with ID {id} not found", 404);
            }

            return ServiceResponse<T>.SuccessResponse(entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching entity {EntityType} with ID: {Id}", typeof(T).Name, id);
            return ServiceResponse<T>.ErrorResponse("Internal server error", 500);
        }
    }

    public virtual async Task<ServiceResponse<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            Logger.LogInformation("Fetching all entities {EntityType}", typeof(T).Name);

            var userId = GetCurrentUserId();
            var userProp = typeof(T).GetProperty("UserId");
            IEnumerable<T> entities;
            if (userProp != null && !string.IsNullOrEmpty(userId))
            {
                entities = await Repository.GetByFilterAsync(e => userProp.GetValue(e)?.ToString() == userId);
            }
            else
            {
                entities = await Repository.GetAllAsync();
            }

            return ServiceResponse<IEnumerable<T>>.SuccessResponse(entities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching all entities {EntityType}", typeof(T).Name);
            return ServiceResponse<IEnumerable<T>>.ErrorResponse("Internal server error", 500);
        }
    }

    public virtual async Task<ServiceResponse<T>> CreateAsync(T entity, string user)
    {
        try
        {
            Logger.LogInformation("Creating new entity {EntityType} for user: {User}", typeof(T).Name, user);

            var userId = GetCurrentUserId();
            var userProp = typeof(T).GetProperty("UserId");
            if (userProp != null && !string.IsNullOrEmpty(userId))
            {
                userProp.SetValue(entity, userId);
            }

            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            var createdEntity = await Repository.CreateAsync(entity);

            Logger.LogInformation("Entity {EntityType} created successfully. ID: {Id}", typeof(T).Name, createdEntity.Id);
            return ServiceResponse<T>.SuccessResponse(createdEntity, "Entity created successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating entity {EntityType} for user: {User}", typeof(T).Name, user);
            return ServiceResponse<T>.ErrorResponse("Internal server error", 500);
        }
    }

    public virtual async Task<ServiceResponse<T>> UpdateAsync(T entity, string user)
    {
        try
        {
            Logger.LogInformation("Updating entity {EntityType} with ID: {Id} for user: {User}", typeof(T).Name, entity.Id, user);

            var existingEntity = await Repository.GetByIdAsync(entity.Id);
            if (existingEntity == null)
            {
                Logger.LogWarning("Entity {EntityType} with ID {Id} not found for update", typeof(T).Name, entity.Id);
                return ServiceResponse<T>.ErrorResponse($"Entity with ID {entity.Id} not found", 404);
            }

            var userId = GetCurrentUserId();
            var userProp = typeof(T).GetProperty("UserId");
            if (userProp != null && userProp.GetValue(existingEntity)?.ToString() != userId)
            {
                return ServiceResponse<T>.ErrorResponse($"Entity with ID {entity.Id} not found", 404);
            }

            var oldValues = System.Text.Json.JsonSerializer.Serialize(existingEntity);

            if (userProp != null)
            {
                userProp.SetValue(entity, userProp.GetValue(existingEntity));
            }
            entity.UpdatedAt = DateTime.UtcNow;
            entity.CreatedAt = existingEntity.CreatedAt;

            var updatedEntity = await Repository.UpdateAsync(entity);



            Logger.LogInformation("Entity {EntityType} updated successfully. ID: {Id}", typeof(T).Name, entity.Id);
            return ServiceResponse<T>.SuccessResponse(updatedEntity, "Entity updated successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating entity {EntityType} with ID: {Id} for user: {User}", typeof(T).Name, entity.Id, user);
            return ServiceResponse<T>.ErrorResponse("Internal server error", 500);
        }
    }

    public virtual async Task<ServiceResponse<bool>> DeleteAsync(string id, string user)
    {
        try
        {
            Logger.LogInformation("Deleting entity {EntityType} with ID: {Id} for user: {User}", typeof(T).Name, id, user);

            var existingEntity = await Repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                Logger.LogWarning("Entity {EntityType} with ID {Id} not found for deletion", typeof(T).Name, id);
                return ServiceResponse<bool>.ErrorResponse($"Entity with ID {id} not found", 404);
            }

            var userId = GetCurrentUserId();
            var userProp = typeof(T).GetProperty("UserId");
            if (userProp != null && userProp.GetValue(existingEntity)?.ToString() != userId)
            {
                return ServiceResponse<bool>.ErrorResponse($"Entity with ID {id} not found", 404);
            }

            var oldValues = System.Text.Json.JsonSerializer.Serialize(existingEntity);

            var result = await Repository.DeleteAsync(id);
            if (result)
            {

                Logger.LogInformation("Entity {EntityType} deleted successfully. ID: {Id}", typeof(T).Name, id);
                return ServiceResponse<bool>.SuccessResponse(true, "Entity deleted successfully");
            }

            return ServiceResponse<bool>.ErrorResponse("Failed to delete entity", 500);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting entity {EntityType} with ID: {Id} for user: {User}", typeof(T).Name, id, user);
            return ServiceResponse<bool>.ErrorResponse("Internal server error", 500);
        }
    }
}
