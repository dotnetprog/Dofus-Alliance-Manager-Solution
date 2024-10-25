using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Services
{
    public class EFSaisonService : ISaisonServiceAsync
    {
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        public EFSaisonService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }
        public async Task ClearRankings(Guid SaisonId)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.SaisonRankings.Where(sr => sr.SaisonId == SaisonId).ExecuteDeleteAsync();

            }
        }

        public async Task<Guid> Create(Saison saison)
        {
            saison.Id = Guid.NewGuid();
            saison.State = SaisonState.Active;
            saison.CreatedOn = DateTime.UtcNow;
            saison.ModifiedOn = DateTime.UtcNow;

            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {

                await dbContext.AddAsync(saison);
                await dbContext.SaveChangesAsync();
            }
            return saison.Id;

        }

        public async Task Delete(Guid SaisonId)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {

                var saison = dbContext.Saisons.Find(SaisonId);
                if (saison == null)
                {
                    throw new KeyNotFoundException(nameof(SaisonId));
                }

                dbContext.Saisons.Remove(saison);
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task<Saison> GetById(Guid AllianceId, Guid Id)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var saison = await dbContext.Saisons.Include(s => s.Rankings).FirstOrDefaultAsync(s => s.AllianceId == AllianceId && s.Id == Id);
                if (saison == null)
                {
                    throw new KeyNotFoundException(nameof(Id));
                }
                return saison;
            }
        }

        public async Task<IReadOnlyCollection<Saison>> GetList(Guid AllianceId)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var saisons = await dbContext.Saisons.Where(s => s.AllianceId == AllianceId).ToListAsync();

                return saisons;
            }
        }

        public async Task<IReadOnlyCollection<SaisonRanking>> GetRankings(Guid AllianceId, Guid SaisonId, bool IncludeMembers = false)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var query = (from sr in dbContext.SaisonRankings
                             join s in dbContext.Saisons
                             on sr.SaisonId equals s.Id
                             where s.AllianceId == AllianceId && sr.SaisonId == SaisonId
                             select sr);
                if (IncludeMembers)
                {
                    query = query.Include(r => r.Member);
                }
                return await query.ToArrayAsync();
            }
        }

        public async Task Update(Saison saison)
        {
            saison.ModifiedOn = DateTime.UtcNow;

            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                dbContext.Saisons.Attach(saison);

                var entry = dbContext.Entry(saison);
                entry.State = EntityState.Modified;
                if (!saison.LadderGeneratedOn.HasValue)
                {
                    entry.Property(p => p.LadderGeneratedOn).IsModified = false;
                }
                entry.Property(p => p.AllianceId).IsModified = false;
                entry.Property(p => p.CreatedById).IsModified = false;
                entry.Property(p => p.CreatedOn).IsModified = false;
                entry.Property(p => p.Description).IsModified = true;
                entry.Property(p => p.Name).IsModified = true;
                entry.Property(p => p.ModifiedById).IsModified = true;
                entry.Property(p => p.ModifiedOn).IsModified = true;
                entry.Property(p => p.StartDate).IsModified = true;
                entry.Property(p => p.EndDate).IsModified = true;
                entry.Property(p => p.SeasonRankingChannelId).IsModified = true;
                entry.Property(p => p.BaremeAttackId).IsModified = true;
                entry.Property(p => p.BaremeDefenseId).IsModified = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateRanking(SaisonRanking ranking)
        {
            ranking.ModifiedOn = DateTime.UtcNow;
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                dbContext.SaisonRankings.Attach(ranking);

                var entry = dbContext.Entry(ranking);
                entry.State = EntityState.Modified;
                entry.Property(p => p.SaisonId).IsModified = false;
                entry.Property(p => p.CreatedById).IsModified = false;
                entry.Property(p => p.CreatedOn).IsModified = false;
                entry.Property(p => p.MemberId).IsModified = false;

                entry.Property(p => p.MontantAtkPepites).IsModified = false;
                entry.Property(p => p.MontantAvAPepites).IsModified = false;
                entry.Property(p => p.MontantDefPepites).IsModified = false;

                entry.Property(p => p.ModifiedById).IsModified = true;
                entry.Property(p => p.ModifiedOn).IsModified = true;
                entry.Property(p => p.MontantTotalPepite).IsModified = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateRankings(IEnumerable<SaisonRanking> rankings)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.AddRangeAsync(rankings);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<SaisonRanking> GetRanking(Guid AllianceId, Guid SaisonRankingId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await (from sr in dbContext.SaisonRankings
                                    join s in dbContext.Saisons
                                    on sr.SaisonId equals s.Id
                                    where s.AllianceId == AllianceId && sr.Id == SaisonRankingId
                                    select sr).Include(sr => sr.Member).FirstOrDefaultAsync();
                return result;
            }
        }

        public async Task<SaisonRanking> GetRanking(Guid AllianceId, Guid SaisonId, string PlayerAlias)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await (from sr in dbContext.SaisonRankings
                                    join s in dbContext.Saisons
                                    on sr.SaisonId equals s.Id
                                    join m in dbContext.Members
                                    on sr.MemberId equals m.Id
                                    where s.AllianceId == AllianceId && sr.SaisonId == SaisonId
                                    && (m.Alias == PlayerAlias || m.DiscordId == PlayerAlias)
                                    select sr).Include(sr => sr.Member).FirstOrDefaultAsync();
                return result;
            }
        }
    }
}
