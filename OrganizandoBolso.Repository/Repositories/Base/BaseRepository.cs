using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrganizandoBolso.Domain.Configuration;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models.Base;
using System.Linq.Expressions;

namespace OrganizandoBolso.Repository.Repositories.Base;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> Collection;
    protected readonly ILogger<BaseRepository<T>> Logger;
    protected readonly MongoDbSettings Settings;

    protected BaseRepository(IOptions<MongoDbSettings> settings, ILogger<BaseRepository<T>> logger, string collectionName)
    {
        Settings = settings.Value;
        Logger = logger;

        var client = new MongoClient(Settings.ConnectionString);
        var database = client.GetDatabase(Settings.DatabaseName);
        Collection = database.GetCollection<T>(collectionName);
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            var result = await Collection.Find(filter).FirstOrDefaultAsync();

            Logger.LogDebug("Entity {EntityType} with ID {Id} fetched: {Found}", typeof(T).Name, id, result != null);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching entity {EntityType} with ID: {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            var result = await Collection.Find(_ => true).ToListAsync();
            Logger.LogDebug("All entities {EntityType} fetched: {Count} found", typeof(T).Name, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching all entities {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetByFilterAsync(Func<T, bool> filter)
    {
        try
        {
            var allEntities = await GetAllAsync();
            var filteredEntities = allEntities.Where(filter).ToList();

            Logger.LogDebug("Filtered entities {EntityType}: {Count} found", typeof(T).Name, filteredEntities.Count);
            return filteredEntities;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error filtering entities {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        try
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }

            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await Collection.InsertOneAsync(entity);

            Logger.LogInformation("Entity {EntityType} created successfully. ID: {Id}", typeof(T).Name, entity.Id);
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating entity {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        try
        {
            entity.UpdatedAt = DateTime.UtcNow;

            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            var update = Builders<T>.Update
                .Set(x => x.UpdatedAt, entity.UpdatedAt);

            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != nameof(BaseEntity.Id) &&
                           p.Name != nameof(BaseEntity.CreatedAt) &&
                           p.Name != nameof(BaseEntity.UpdatedAt));

            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                if (value != null)
                {
                    update = update.Set(property.Name, value);
                }
            }

            var result = await Collection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                Logger.LogInformation("Entity {EntityType} updated successfully. ID: {Id}", typeof(T).Name, entity.Id);
                return await GetByIdAsync(entity.Id) ?? entity;
            }
            else
            {
                Logger.LogWarning("Entity {EntityType} with ID {Id} was not updated", typeof(T).Name, entity.Id);
                return entity;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating entity {EntityType} with ID: {Id}", typeof(T).Name, entity.Id);
            throw;
        }
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            var result = await Collection.DeleteOneAsync(filter);

            var deleted = result.DeletedCount > 0;
            Logger.LogInformation("Entity {EntityType} with ID {Id} deleted: {Deleted}", typeof(T).Name, id, deleted);

            return deleted;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting entity {EntityType} with ID: {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
    {
        try
        {
            var skip = (page - 1) * pageSize;
            var result = await Collection.Find(_ => true)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            Logger.LogDebug("Paged entities {EntityType}: page {Page}, size {PageSize}, found {Count}",
                typeof(T).Name, page, pageSize, result.Count);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching paged entities {EntityType}: page {Page}, size {PageSize}",
                typeof(T).Name, page, pageSize);
            throw;
        }
    }

    protected FilterDefinition<T> BuildFilter(Expression<Func<T, bool>> expression)
    {
        return Builders<T>.Filter.Where(expression);
    }

    protected SortDefinition<T> BuildSort(Expression<Func<T, object>> expression, bool ascending = true)
    {
        return ascending
            ? Builders<T>.Sort.Ascending(expression)
            : Builders<T>.Sort.Descending(expression);
    }
}
