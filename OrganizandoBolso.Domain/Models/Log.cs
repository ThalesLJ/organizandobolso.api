using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Models;

[SwaggerSchema(Description = "Audit log model")]
public class Log : BaseEntity
{
    [BsonElement("action")]
    [SwaggerSchema(Description = "Action performed")]
    public string Action { get; set; } = string.Empty;

    [BsonElement("message")]
    [SwaggerSchema(Description = "Descriptive log message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("oldValues")]
    [SwaggerSchema(Description = "Previous values (JSON)")]
    public string? OldValues { get; set; }

    [BsonElement("newValues")]
    [SwaggerSchema(Description = "New values (JSON)")]
    public string? NewValues { get; set; }
}
