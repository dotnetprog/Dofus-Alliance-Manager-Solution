using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Services
{
    public class EFAvaService : IAvAService
    {
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;

        public EFAvaService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }
        public async Task<Guid> CreateAvA(AvA ava)
        {

            ava.CreatedOn = DateTime.UtcNow;
            ava.Id = Guid.NewGuid();
            ava.State = AvaState.Open;
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.AddAsync(ava);
                await dbContext.SaveChangesAsync();
            }

            return ava.Id;
        }

        public async Task DeleteAvA(Guid AllianceId, Guid Id)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var existingAva = await dbContext.AvA.Where(a => a.AllianceId == AllianceId && a.Id == Id).FirstOrDefaultAsync();
                if (existingAva == null)
                {
                    return;
                }

                dbContext.AvA.Remove(existingAva);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task EditAvAMember(AvaMember member)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var existingAva = await dbContext.AvaMembers.Where(a => a.Id == member.Id).FirstOrDefaultAsync();
                if (existingAva == null)
                {
                    throw new KeyNotFoundException($"{nameof(AvaMember)} With Id = {member.Id} does not exist.");
                }
                if (member.ValidationState.HasValue)
                {
                    existingAva.ValidationState = member.ValidationState;
                    existingAva.ValidatedById = member.ValidatedById;
                }
                if (!string.IsNullOrEmpty(member.ImageUrl))
                {
                    existingAva.ImageUrl = member.ImageUrl;
                }
                if (member.MontantPepites.HasValue)
                {
                    existingAva.MontantPepites = member.MontantPepites;
                }
                if (member.DiscordMessageId != default(ulong))
                {
                    existingAva.DiscordMessageId = member.DiscordMessageId;
                }
                dbContext.AvaMembers.Update(existingAva);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<AvA?> GetAvA(Guid AllianceId, Guid Id, bool includeMembers = false)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var query = dbContext.AvA.Where(a => a.AllianceId == AllianceId && a.Id == Id);
                if (includeMembers)
                {
                    query = query.Include(a => a.Members).Include(a => a.ClosedBy).Include(a => a.CreatedBy);
                }

                var ava = await query.FirstOrDefaultAsync();
                return ava;
            }
        }

        public async Task<AvA?> GetAvA(Guid AllianceId, ulong discordChannelId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var ava = await dbContext.AvA.Where(a => a.AllianceId == AllianceId && a.DiscordThreadChannelId == discordChannelId).FirstOrDefaultAsync();
                return ava;
            }

        }

        public async Task<IReadOnlyCollection<AvA>> GetAvAList(Guid AllianceId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var avalist = await dbContext.AvA.Where(a => a.AllianceId == AllianceId).ToListAsync();
                return avalist;
            }
        }

        public async Task<AvaMember?> GetMember(Guid memberid)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var member = await dbContext.AvaMembers.Where(a => a.Id == memberid).Include(a => a.AvA).FirstOrDefaultAsync();
                return member;
            }
        }

        public async Task<AvaMember?> GetMember(Guid AvaId, Guid AllianceMemberId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var member = await dbContext.AvaMembers.Where(a => a.AvaId == AvaId && a.MemberId == AllianceMemberId).FirstOrDefaultAsync();
                return member;
            }
        }

        public async Task<AvaMember?> GetMember(Guid AvaId, ulong DiscordMessageId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var members = await dbContext.AvaMembers.Where(a => a.AvaId == AvaId && a.DiscordMessageId == DiscordMessageId).FirstOrDefaultAsync();
                return members;
            }
        }

        public async Task<IReadOnlyCollection<AvaMember>> GetMembers(Guid AvaId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var members = await dbContext.AvaMembers.Where(a => a.AvaId == AvaId).ToArrayAsync();
                return members;
            }
        }

        public async Task PayMembers(Guid Id, int MontantPepites)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.AvaMembers
                    .Where(m => m.AvaId == Id && m.ValidationState != AvAValidationState.Rejected)
                    .ExecuteUpdateAsync(p => p.SetProperty((m) => m.MontantPepites, (m) => MontantPepites));
            }
        }

        public async Task<Guid> RegisterAvAMember(AvaMember member)
        {
            member.Id = Guid.NewGuid();
            member.CreatedOn = DateTime.UtcNow;
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.AvaMembers.AddAsync(member);
                await dbContext.SaveChangesAsync();
            }
            return member.Id;
        }

        public async Task RemoveAvAMember(Guid memberid)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var existingMember = dbContext.AvaMembers.Find(memberid);
                if (existingMember == null)
                {
                    return;
                }
                dbContext.AvaMembers.Remove(existingMember);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAva(AvA ava)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var existingAvA = dbContext.AvA.Find(ava.Id);
                if (existingAvA == null)
                {
                    throw new KeyNotFoundException($"{nameof(AvA)} With Id = {ava.Id} does not exist.");
                }
                bool isModified = false;
                if (ava.ResultState.HasValue)
                {
                    existingAvA.ResultState = ava.ResultState.Value;
                    existingAvA.MontantPepitesObtenu = ava.MontantPepitesObtenu;
                    existingAvA.MontantPepitesTotal = ava.MontantPepitesTotal;
                    isModified = true;
                }
                if (ava.MontantPayeFixe.HasValue)
                {
                    existingAvA.MontantPayeFixe = ava.MontantPayeFixe;
                }
                if (ava.State.HasValue && ava.State != existingAvA.State)
                {
                    existingAvA.State = ava.State;
                    existingAvA.ClosedOn = DateTime.UtcNow;
                    existingAvA.ClosedById = ava.ClosedById;
                    isModified = true;
                }
                if (!isModified) { return; }
                dbContext.Update(existingAvA);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
