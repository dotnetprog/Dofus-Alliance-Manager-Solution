using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.Domain.JsonData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Data.EntityFramework.Services
{
    public class EFMembreService : IMembreService
    {
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        public EFMembreService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }
        public async Task<Guid> AddPseudo(AnkamaPseudo pseudo)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var existing = await dbContext.AnkamaPseudos
                    .Where(ak => ak.AllianceMemberId == pseudo.AllianceMemberId &&
                                 ak.Pseudo == pseudo.Pseudo.ToLowerInvariant()).FirstOrDefaultAsync();

                if (existing != null)
                {
                    existing.LastRefreshedOn = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(pseudo.PseudoData))
                    {
                        existing.PseudoData = pseudo.PseudoData.ToLowerInvariant();
                    }
                    existing.AnkadexUrl = pseudo.AnkadexUrl;
                    existing.CreatedById = pseudo.CreatedById;
                    
                    dbContext.Update(existing);
                    await dbContext.SaveChangesAsync();
                    return existing.Id;
                }
                else
                {
                    pseudo.Id = Guid.NewGuid();
                    pseudo.LastRefreshedOn = DateTime.UtcNow;
                    pseudo.PseudoData = pseudo.PseudoData.ToLowerInvariant();
                    pseudo.Pseudo = pseudo.Pseudo.ToLowerInvariant();
                    await dbContext.AnkamaPseudos.AddAsync(pseudo);
                    await dbContext.SaveChangesAsync();
                    return pseudo.Id;
                }
            }
        }

        public async Task DeletePseudo(Guid MemberId, string ankamapseudo)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var pseudo = await dbContext.AnkamaPseudos.Where(ap => ap.AllianceMemberId == MemberId && ap.Pseudo == ankamapseudo.ToLowerInvariant()).FirstOrDefaultAsync();
                if(pseudo == null)
                {
                    return;
                }
                dbContext.AnkamaPseudos.Remove(pseudo);
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task<IReadOnlyCollection<AnkamaPseudo>> GetPseudos(Guid AllianceMemberId)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var pseudos = dbContext.AnkamaPseudos.Where(ap => ap.AllianceMemberId == AllianceMemberId).Include(ap => ap.CreatedBy).ToList();
                return pseudos;
            }
        }

        public async Task<IReadOnlyCollection<AllianceMember>> RechercheParPersonnage(Guid AllianceId, string Personnage, string Serveur)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var results = await dbContext.AnkamaPseudos
                    .Join(dbContext.Members,
                      ap => ap.AllianceMemberId,
                      m => m.Id,
                      (ap, m) => new { ap = ap, m = m })
                    .Where(ap => ap.m.AllianceId == AllianceId && EF.Functions.Like(ap.ap.PseudoData, $"%{Personnage.ToLowerInvariant()}%")).ToListAsync();
                var resultsWithServers = results.Where(r => JsonConvert.DeserializeObject<AnkamaPseudoData[]>(r.ap.PseudoData)
                .Any(apd => apd.NomPersonnage == Personnage.ToLowerInvariant() && apd.Serveur == Serveur.ToLowerInvariant())).Select(r => r.m).ToList();
                
                return resultsWithServers;
            }
        }

        public async Task<IReadOnlyCollection<AllianceMember>> RechercheParPseudo(Guid AllianceId, string Pseudo)
        {
            using (var dbContext = await _dbContextBuilder.CreateDbContextAsync())
            {
                var members = dbContext.AnkamaPseudos
                    .Join(dbContext.Members,
                      ap => ap.AllianceMemberId,
                      m => m.Id,
                      (ap,m) => new { ap = ap,m = m })
                    .Where(ap => ap.m.AllianceId == AllianceId && ap.ap.Pseudo == Pseudo.ToLowerInvariant()).Select(ap => ap.m).ToList();
                return members;
            }
        }
    }
}
