using Alachisoft.NCache.EntityFrameworkCore;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Services
{
    public class EFBaremeService : IBaremeServiceAsync
    {
        private readonly CachingOptions cachingOptions = new CachingOptions
        {
            StoreAs = StoreAs.SeparateEntities
        };
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        public EFBaremeService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }

        public async Task<Guid> CreateBareme(Bareme bareme)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                bareme.Id = Guid.NewGuid();
                dbContext.Baremes.Add(bareme);
                await dbContext.SaveChangesAsync();
                return bareme.Id;
            }
        }

        public async Task<Guid> CreateBaremeDetail(BaremeDetail detail)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                detail.Id = Guid.NewGuid();
                dbContext.BaremeDetails.Add(detail);
                await dbContext.SaveChangesAsync();
                return detail.Id;
            }
        }

        public async Task DeleteBareme(Guid BaremeId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                await dbContext.AllianceConfigurations.Where(c => c.DefaultBaremeAttaqueId == BaremeId)
                    .ExecuteUpdateAsync(p => p.SetProperty(c => c.DefaultBaremeAttaqueId, c => null));
                await dbContext.AllianceConfigurations.Where(c => c.DefaultBaremeDefenseId == BaremeId)
                    .ExecuteUpdateAsync(p => p.SetProperty(c => c.DefaultBaremeDefenseId, c => null));

                await dbContext.Enemies.Where(e => e.BaremeDefenseId == BaremeId).ExecuteUpdateAsync(p => p.SetProperty(c => c.BaremeDefenseId, c => null));
                await dbContext.Enemies.Where(e => e.BaremeAttaqueId == BaremeId).ExecuteUpdateAsync(p => p.SetProperty(c => c.BaremeAttaqueId, c => null));

                var bareme = await dbContext.Baremes.FindAsync(BaremeId);
                if (bareme == null)
                    return;
                dbContext.Baremes.Remove(bareme);
                dbContext.GetCache().Remove(bareme);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteBaremeDetail(Guid detailid)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var baremedetail = await dbContext.BaremeDetails.FindAsync(detailid);
                if (baremedetail == null)
                    return;
                dbContext.BaremeDetails.Remove(baremedetail);
                dbContext.GetCache().Remove(baremedetail);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<Bareme?> GetBareme(Guid AllianceId, Guid BaremeId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var baremeQuery = await dbContext.Baremes.Include(b => b.Details)
                    .Include(b => b.EnemiesDefense)
                    .Include(b => b.EnemiesAttaque)
                    .Where(b => b.Id == BaremeId && b.AllianceId == AllianceId)
                    .FromCacheAsync(cachingOptions);
                var bareme = baremeQuery.FirstOrDefault();
                return bareme;
            }
        }

        public async Task<IReadOnlyCollection<Bareme>> GetBaremes(Guid AllianceId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var baremeQuery = await dbContext.Baremes.Include(b => b.Details)
                    .Include(b => b.EnemiesDefense)
                    .Include(b => b.EnemiesAttaque)
                    .Where(b => b.AllianceId == AllianceId)
                    .FromCacheAsync(cachingOptions);
                var bareme = baremeQuery.ToList();
                return bareme;
            }
        }

        public async Task UpdateBareme(Bareme bareme)
        {
            try
            {
                var existingBareme = await GetBareme(bareme.AllianceId, bareme.Id);
                var defenseEnemiesToRemove = existingBareme.EnemiesDefense.Where(ed => !bareme.EnemiesDefense.Any(be => be.Id == ed.Id)).Select(e => e.Id).ToList();
                var attauqeEnemiesToRemove = existingBareme.EnemiesAttaque.Where(ed => !bareme.EnemiesAttaque.Any(be => be.Id == ed.Id)).Select(e => e.Id).ToList();
                using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
                {

                    dbContext.Baremes.Attach(bareme);

                    var entry = dbContext.Entry(bareme);
                    entry.State = EntityState.Modified;
                    entry.Property(p => p.AllianceId).IsModified = false;
                    entry.Collection(p => p.EnemiesAttaque).IsModified = true;
                    entry.Collection(p => p.EnemiesDefense).IsModified = true;
                    foreach (var detail in bareme.Details)
                    {
                        var entityEntry = dbContext.Entry(detail);
                        entityEntry.State = EntityState.Modified;

                        entityEntry.Property(navProp => navProp.BaremeId).IsModified = false;
                        entityEntry.Property(navProp => navProp.NbEnemie).IsModified = false;
                        entityEntry.Property(navProp => navProp.NbAllie).IsModified = false;
                    }
                    await dbContext.SaveChangesAsync();
                    dbContext.GetCache().Remove(bareme);

                    await dbContext.Enemies.Where(e => defenseEnemiesToRemove.Contains(e.Id)).ExecuteUpdateAsync(e => e.SetProperty(en => en.BaremeDefenseId, en => null));

                    await dbContext.Enemies.Where(e => attauqeEnemiesToRemove.Contains(e.Id)).ExecuteUpdateAsync(e => e.SetProperty(en => en.BaremeAttaqueId, en => null));

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
