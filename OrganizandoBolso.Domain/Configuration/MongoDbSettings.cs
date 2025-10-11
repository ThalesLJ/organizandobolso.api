namespace OrganizandoBolso.Domain.Configuration;

public class MongoDbSettings
{
    private string _connectionString = string.Empty;
    public string ConnectionString
    {
        get => _connectionString;
        set
        {
            _connectionString = value;
        }
    }
    public string DatabaseName { get; set; } = "OrganizandoBolso";
    public CollectionNames CollectionNames { get; set; } = new();
}

public class CollectionNames
{
    public string Budgets { get; set; } = "Budgets";
    public string Expenses { get; set; } = "Expenses";
    public string Logs { get; set; } = "Logs";
    public string Settings { get; set; } = "Settings";
}
