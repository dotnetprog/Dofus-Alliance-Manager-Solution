using Alachisoft.NCache.EntityFrameworkCore;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Services
{
    public class EFAlertService : IAlertServiceAsync
    {
        private readonly CachingOptions cachingOptions = new CachingOptions
        {
            StoreAs = StoreAs.SeparateEntities
        };
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        public EFAlertService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }
        public async Task<Alert> CreateAlert(string message, Guid CreatedBy, Guid AllianceId, int audienceCount)
        {
            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                AudienceCount = audienceCount,
                CreatedByMemberId = CreatedBy,
                CreatedOn = DateTime.UtcNow,
                Message = message
            };

            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.AddAsync(alert);
                await dbContext.SaveChangesAsync();
            }
            return alert;
        }

        public async Task<Alert?> GetAlert(Guid AlertId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var alert = await dbContext.Alerts.FindAsync(AlertId);
                return alert;
            }
        }

        public async Task<IReadOnlyCollection<Alert>> GetAlerts(Guid AllianceId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var query = await dbContext.Alerts.Include(a => a.CreatedBy)
                                            .Where(a => a.CreatedBy.AllianceId == AllianceId)
                                            .OrderByDescending(a => a.CreatedOn)
                                            .FromCacheAsync(new CachingOptions
                                            {
                                                QueryIdentifier = $"GetAlerts-{AllianceId}",
                                                StoreAs = StoreAs.Collection
                                            });

                return query.ToList();
            }
        }

        public async Task<Alert?> GetMostRecentAlert(Guid AllianceId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var query = await dbContext.Alerts.Include(a => a.CreatedBy)
                                            .Where(a => a.CreatedBy.AllianceId == AllianceId)
                                            .OrderByDescending(a => a.CreatedOn)
                                            .FromCacheAsync(new CachingOptions
                                            {
                                                QueryIdentifier = $"GetMostRecentAlert-{AllianceId}",
                                                StoreAs = StoreAs.Collection
                                            });

                return query.FirstOrDefault();
            }
        }
    }
}
