using Alachisoft.NCache.EntityFrameworkCore;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.Domain.Enums;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace DAM.Data.EntityFramework.Services
{
    public class EFAllianceManagementService : IAllianceManagementServiceAsync
    {
        private readonly CachingOptions cachingOptions = new CachingOptions
        {
            StoreAs = StoreAs.SeparateEntities
        };
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;
        public EFAllianceManagementService(IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _dbContextBuilder = dbContextBuilder;
        }
        public async Task<AllianceMember> GetOrCreateAllianceMember(Guid AllianceId, IGuildUser user)
        {
            try
            {

                var avatar = string.Empty;
                try
                {
                    var displayAvatar = user.GetDisplayAvatarUrl();
                    avatar = string.IsNullOrEmpty(displayAvatar) ? user.GetDefaultAvatarUrl() : displayAvatar;
                }
                catch
                {
                    avatar = string.Empty;
                }


                var member = new AllianceMember()
                {
                    Alias = user.Username,
                    DiscordId = user.Id.ToString(),
                    AllianceId = AllianceId,
                    Nickname = string.IsNullOrEmpty(user.Nickname) ? user.DisplayName : user.Nickname,
                    DonotAlert = false,
                    AvatarUrl = avatar
                };
                member.Id = await this.AddMember(member);
                return member;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Guid> AddEnemy(AllianceEnemy enemy)
        {
            if (enemy.AllianceId == Guid.Empty)
            {
                throw new ArgumentException(nameof(enemy.AllianceId));
            }
            if (string.IsNullOrWhiteSpace(enemy.Name))
            {
                throw new ArgumentException(nameof(enemy.Name));
            }

            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                enemy.Id = Guid.NewGuid();
                await dbContext.Enemies.AddAsync(enemy);
                await dbContext.SaveChangesAsync();
                return enemy.Id;
            }

        }

        public async Task<Guid> AddMember(AllianceMember member)
        {
            if (!member.AllianceId.HasValue)
            {
                throw new ArgumentNullException(nameof(member.AllianceId));
            }
            if (string.IsNullOrWhiteSpace(member.DiscordId))
            {
                throw new ArgumentNullException(nameof(member.DiscordId));
            }
            var existingMember = await this.GetMember(member.AllianceId.Value, member.DiscordId);
            if (existingMember != null)
            {
                //Reactivate user if was inactive
                if (existingMember.State == MemberState.Inactive ||
                    existingMember.Nickname != member.Nickname ||
                    (existingMember.AvatarUrl != member.AvatarUrl && !string.IsNullOrEmpty(member.AvatarUrl)))
                {
                    if (existingMember.State == MemberState.Inactive)
                    {
                        existingMember.State = MemberState.Active;
                        existingMember.Status = MemberStatus.Active;
                    }
                    if (existingMember.Nickname != member.Nickname)
                    {
                        existingMember.Nickname = member.Nickname;
                    }
                    if (existingMember.AvatarUrl != member.AvatarUrl && !string.IsNullOrEmpty(member.AvatarUrl))
                    {
                        existingMember.AvatarUrl = member.AvatarUrl;
                    }

                    await this.UpdateMember(existingMember);
                }
                return existingMember.Id;

            }

            member.Id = Guid.NewGuid();

            member.ModifiedOn = DateTime.UtcNow;
            member.CreatedOn = DateTime.UtcNow;
            using (var dbcontext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbcontext.Members.AddAsync(member);
                await dbcontext.SaveChangesAsync();
            }
            return member.Id;
        }

        public async Task DeleteEnemy(Guid GuildId, Guid EnemyId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var Enemy = await dbContext.Enemies.Where(e => e.AllianceId == GuildId && e.Id == EnemyId).FirstOrDefaultAsync();
                if (Enemy == null)
                { return; }

                var relatedScreenPosts = await dbContext.ScreenPosts.Where(sp => sp.AllianceEnemyId == EnemyId)
                    .ExecuteUpdateAsync(e => e.SetProperty(en => en.AllianceEnemyId, en => null));


                dbContext.Enemies.Remove(Enemy);
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task<Alliance?> GetAlliance(Guid AllianceId)
        {

            using (var dbcontext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await dbcontext.Alliances
                    .Include(a => a.AllianceConfiguration)
                    .Where(a => a.Id == AllianceId).FromCacheAsync(cachingOptions);

                return result.FirstOrDefault();
            }

        }

        public async Task<Alliance?> GetAlliance(string DiscordGuildId)
        {

            using (var dbcontext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await dbcontext.Alliances
                    .Include(a => a.AllianceConfiguration)
                    .Where(a => a.DiscordGuildId == DiscordGuildId).FromCacheAsync(cachingOptions);

                return result.FirstOrDefault();

            }

        }

        public async Task<AllianceConfiguration?> GetAllianceConfiguration(Guid GuildId)
        {
            using (var dbcontext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await dbcontext.Alliances.Where(a => a.Id == GuildId)
                    .Include(a => a.AllianceConfiguration)
                    .Where(ac => ac.Id == GuildId).FromCacheAsync(cachingOptions);
                return result.FirstOrDefault()?.AllianceConfiguration;
            }
        }

        public async Task<IReadOnlyCollection<AllianceMember>> GetAllMembers(Guid AllianceId)
        {
            using (var dbcontext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                return await dbcontext.Members.Where(m => m.AllianceId == AllianceId).ToArrayAsync();
            }


        }

        public async Task<IReadOnlyCollection<AllianceEnemy>> GetEnemies(Guid GuildId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                return await dbContext.Enemies.Where(e => e.AllianceId == GuildId).ToArrayAsync();
            }
        }

        public async Task<AllianceEnemy?> GetEnemy(Guid GuildId, Guid EnemyId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                return await dbContext.Enemies.Where(e => e.AllianceId == GuildId && e.Id == EnemyId).FirstOrDefaultAsync();
            }
        }

        public async Task<AllianceMember?> GetMember(Guid MemberId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await dbContext.Members.Where(m => m.Id == MemberId)
                    .FromCacheAsync(cachingOptions);
                return result.FirstOrDefault();
            }

        }

        public async Task<AllianceMember?> GetMember(Guid AllianceId, string discordId)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var result = await dbContext.Members.Where(m => m.DiscordId == discordId && m.AllianceId == AllianceId)
                    .FromCacheAsync(cachingOptions);
                return result.FirstOrDefault();

            }

        }

        public async Task<Alliance> RegisterAlliance(string discordguildid, string alliancename)
        {
            var alliance = new Alliance();
            alliance.Id = Guid.NewGuid();
            alliance.DiscordGuildId = discordguildid;
            alliance.Alias = alliancename;
            alliance.RegisteredOn = DateTime.UtcNow;
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                await dbContext.Alliances.AddAsync(alliance);
                await dbContext.SaveChangesAsync();
            }

            return alliance;
        }

        public async Task RemoveMember(Guid GuildId, string discorduserid)
        {
            var member = await this.GetMember(GuildId, discorduserid);
            if (member == null)
            {
                return;
            }
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                member.State = MemberState.Inactive;
                member.Status = MemberStatus.Inactive;
                member.ModifiedOn = DateTime.UtcNow;
                dbContext.Members.Update(member);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveMember(Guid MemberId)
        {
            var member = await this.GetMember(MemberId);
            if (member == null)
            {
                return;
            }
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                member.State = MemberState.Inactive;
                member.Status = MemberStatus.Inactive;
                member.ModifiedOn = DateTime.UtcNow;
                dbContext.Members.Update(member);
                await dbContext.SaveChangesAsync();
                var cache = dbContext.GetCache();
                cache.Remove(member);
            }
        }

        public async Task<AllianceConfiguration> UpdateAllianceConfiguration(Guid GuildId, AllianceConfiguration allianceConfiguration)
        {
            var alliance = await this.GetAlliance(GuildId);
            if (alliance == null)
            {
                throw new KeyNotFoundException($"No {nameof(Alliance)} exists for this Id = {nameof(GuildId)}");
            }

            allianceConfiguration.Id = alliance.AllianceConfigurationId ?? Guid.Empty;
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var cache = dbContext.GetCache();
                if (!alliance.AllianceConfigurationId.HasValue)
                {
                    allianceConfiguration.Id = Guid.NewGuid();
                    dbContext.AllianceConfigurations.Add(allianceConfiguration);


                    alliance.AllianceConfigurationId = allianceConfiguration.Id;
                    dbContext.Alliances.Update(alliance);
                    cache.Remove(alliance);
                }
                else
                {
                    var existing = dbContext.AllianceConfigurations.Find(allianceConfiguration.Id);

                    existing.Rapport_DiscordChannelId = allianceConfiguration.Rapport_DiscordChannelId;
                    existing.AtkScreen_DiscordChannelId = allianceConfiguration.AtkScreen_DiscordChannelId;
                    existing.DefScreen_DiscordChannelId = allianceConfiguration.DefScreen_DiscordChannelId;
                    existing.IsScreenPrepaRequired = allianceConfiguration.IsScreenPrepaRequired;
                    existing.ScreenApproverRoleId = allianceConfiguration.ScreenApproverRoleId;
                    existing.IsAllianceEnemyRequired = allianceConfiguration.IsAllianceEnemyRequired;

                    if (allianceConfiguration.AutoValidateNoDef.HasValue)
                    {
                        existing.AutoValidateNoDef = allianceConfiguration.AutoValidateNoDef;
                    }

                    if (allianceConfiguration.AllowSeasonOverlap.HasValue)
                    {
                        existing.AllowSeasonOverlap = allianceConfiguration.AllowSeasonOverlap;
                    }
                    if (allianceConfiguration.Ava_DiscordForumChannelId.HasValue)
                    {
                        existing.Ava_DiscordForumChannelId = allianceConfiguration.Ava_DiscordForumChannelId;
                    }
                    if (allianceConfiguration.DefaultBaremeDefenseId.HasValue)
                    {
                        existing.DefaultBaremeDefenseId = allianceConfiguration.DefaultBaremeDefenseId;
                    }
                    if (allianceConfiguration.DefaultBaremeAttaqueId.HasValue)
                    {
                        existing.DefaultBaremeAttaqueId = allianceConfiguration.DefaultBaremeAttaqueId;
                    }
                    if (allianceConfiguration.DefaultSeasonRankingChannelId.HasValue)
                    {
                        existing.DefaultSeasonRankingChannelId = allianceConfiguration.DefaultSeasonRankingChannelId;
                    }
                    if (!string.IsNullOrWhiteSpace(allianceConfiguration.BehaviorScreenConfigJSONData))
                    {
                        existing.BehaviorScreenConfigJSONData = allianceConfiguration.BehaviorScreenConfigJSONData;
                    }
                    if (allianceConfiguration.BotScreenBehaviorType.HasValue)
                    {
                        existing.BotScreenBehaviorType = allianceConfiguration.BotScreenBehaviorType;
                        if (allianceConfiguration.BotScreenBehaviorType == BotScreenBehaviorType.Commands)
                        {
                            existing.BehaviorScreenConfigJSONData = null;
                        }
                    }
                    dbContext.AllianceConfigurations.Update(existing);
                    cache.Remove(existing);
                }
                await dbContext.SaveChangesAsync();
                return allianceConfiguration;
            }

        }

        public async Task UpdateEnemy(AllianceEnemy enemy)
        {
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                var existingEnemy = await dbContext.Enemies.FindAsync(enemy.Id);
                if (enemy.AllianceId != existingEnemy.AllianceId)
                {
                    throw new InvalidOperationException("AllianceId of an enemy cannot be updated.");
                }
                if (existingEnemy == null)
                {
                    throw new KeyNotFoundException($"{nameof(AllianceEnemy)} With Id = {enemy.Id} does not exist.");
                }
                dbContext.Enemies.Entry(existingEnemy).CurrentValues.SetValues(enemy);
                await dbContext.SaveChangesAsync();

            }
        }

        public async Task UpdateMember(AllianceMember member)
        {
            var existingMember = await this.GetMember(member.Id);
            if (existingMember == null)
            {
                throw new KeyNotFoundException($"{nameof(AllianceMember)} With Id = {member.Id} does not exist.");
            }
            member.ModifiedOn = DateTime.UtcNow;
            using (var dbContext = await this._dbContextBuilder.CreateDbContextAsync())
            {
                dbContext.Attach(existingMember);
                dbContext.Members.Entry(existingMember).CurrentValues.SetValues(member);
                await dbContext.SaveChangesAsync();
                dbContext.GetCache().Remove(existingMember);
            }


        }
    }
}
