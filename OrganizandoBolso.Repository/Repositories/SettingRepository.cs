using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganizandoBolso.Domain.Configuration;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Domain.Models;
using OrganizandoBolso.Repository.Repositories.Base;

namespace OrganizandoBolso.Repository.Repositories;

public class SettingRepository : BaseRepository<Setting>, ISettingRepository
{
    public SettingRepository(IOptions<MongoDbSettings> settings, ILogger<SettingRepository> logger)
        : base(settings, logger, new MongoDbSettings().CollectionNames.Settings)
    {
    }
}
