using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Models;

[SwaggerSchema(Description = "Key-Value application setting")]
public class Setting : BaseEntity
{
    [BsonElement("name")]
    [SwaggerSchema(Description = "Setting name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("value")]
    [SwaggerSchema(Description = "Setting value")]
    public string Value { get; set; } = string.Empty;
}
