using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace OrganizandoBolso.Domain.Models.Base;

[SwaggerSchema(Description = "Base entity with common properties")]
public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerSchema(Description = "Unique entity identifier")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("created_at")]
    [SwaggerSchema(Description = "Creation date and time")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    [SwaggerSchema(Description = "Last update date and time")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
