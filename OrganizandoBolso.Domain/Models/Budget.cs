using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Models;

[SwaggerSchema(Description = "Budget model")]
public class Budget : BaseEntity
{
    [BsonElement("user_id")]
    [SwaggerSchema(Description = "Owner of the Budget")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("name")]
    [SwaggerSchema(Description = "Budget name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("amout")]
    [SwaggerSchema(Description = "Budget amount")]
    public decimal Amount { get; set; }

    [BsonElement("icon")]
    [SwaggerSchema(Description = "Budget representative icon")]
    public string Icon { get; set; } = string.Empty;

    [BsonElement("color")]
    [SwaggerSchema(Description = "Budget representative color")]
    public string Color { get; set; } = string.Empty;
}
