using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using OrganizandoBolso.Domain.Configuration;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Repository.Repositories.Base;

namespace OrganizandoBolso.Repository.Repositories;

public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
{
    public BudgetRepository(IOptions<MongoDbSettings> settings, ILogger<BudgetRepository> logger)
        : base(settings, logger, new MongoDbSettings().CollectionNames.Budgets)
    {
    }
}
