using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using OrganizandoBolso.Domain.Configuration;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Repository.Repositories.Base;

namespace OrganizandoBolso.Repository.Repositories;

public class LogRepository : BaseRepository<Log>, ILogRepository
{
    public LogRepository(IOptions<MongoDbSettings> settings, ILogger<LogRepository> logger)
        : base(settings, logger, new MongoDbSettings().CollectionNames.Logs)
    {
    }
}
