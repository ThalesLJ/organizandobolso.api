using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using OrganizandoBolso.Domain.Models.Base;

namespace OrganizandoBolso.Domain.Models;

[SwaggerSchema(Description = "Expense model")]
public class Expense : BaseEntity
{
    [BsonElement("user_id")]
    [SwaggerSchema(Description = "User who performed the last update")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("budget_id")]
    [SwaggerSchema(Description = "Budget id")]
    public string BudgetId { get; set; } = string.Empty;

    [BsonElement("name")]
    [SwaggerSchema(Description = "Expense name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("amount")]
    [SwaggerSchema(Description = "Expense amount")]
    public decimal Amount { get; set; }

    [BsonElement("description")]
    [SwaggerSchema(Description = "Detailed expense description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("color")]
    [SwaggerSchema(Description = "Expense representative color")]
    public string Color { get; set; } = string.Empty;
}
