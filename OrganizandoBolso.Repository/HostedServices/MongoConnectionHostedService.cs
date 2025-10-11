using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OrganizandoBolso.Domain.Configuration;

namespace OrganizandoBolso.Repository.HostedServices;

public class MongoConnectionHostedService : IHostedService
{
    private readonly IOptions<MongoDbSettings> _settings;
    private readonly ILogger<MongoConnectionHostedService> _logger;

    public MongoConnectionHostedService(IOptions<MongoDbSettings> settings, ILogger<MongoConnectionHostedService> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _settings.Value.ConnectionString;
        var databaseName = _settings.Value.DatabaseName;

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        await database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
        _logger.LogInformation("Connected to MongoDB. Database: {Database}", databaseName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
