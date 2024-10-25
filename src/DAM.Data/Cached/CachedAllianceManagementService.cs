using DAM.Core.Abstractions.Services;
using Discord;
using Microsoft.Extensions.Caching.Memory;

namespace DAM.Data.Cached
{
    public class CachedAllianceManagementService : IAllianceManagementServiceAsync
    {
        private readonly IMemoryCache _cache;
        private readonly IAllianceManagementServiceAsync _service;
        private readonly TimeSpan _defaultDelay;

        public CachedAllianceManagementService(IMemoryCache cache, IAllianceManagementServiceAsync service)
        {
            _cache = cache;
            _service = service;
            _defaultDelay = TimeSpan.FromHours(1);
        }

        public Task<Guid> AddEnemy(DAM.Domain.Entities.AllianceEnemy enemy)
        {
            return _service.AddEnemy(enemy);
        }

        public Task<Guid> AddMember(DAM.Domain.Entities.AllianceMember member)
        {
            return _service.AddMember(member);
        }

        public async Task DeleteEnemy(Guid GuildId, Guid EnemyId)
        {
            await _service.DeleteEnemy(GuildId, EnemyId);
        }

        public async Task<DAM.Domain.Entities.Alliance?> GetAlliance(Guid AllianceId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetAlliance.{AllianceId.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetAlliance(AllianceId);

            });
            return results;
        }

        public async Task<DAM.Domain.Entities.Alliance?> GetAlliance(string DiscordGuildId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetAlliance.{DiscordGuildId}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetAlliance(DiscordGuildId);

            });
            return results;
        }

        public async Task<DAM.Domain.Entities.AllianceConfiguration?> GetAllianceConfiguration(Guid GuildId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetAllianceConfiguration.{GuildId.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetAllianceConfiguration(GuildId);

            });
            return results;

        }

        public async Task<IReadOnlyCollection<DAM.Domain.Entities.AllianceMember>> GetAllMembers(Guid AllianceId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetAllMembers.{AllianceId.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetAllMembers(AllianceId);

            });
            return results;
        }

        public async Task<IReadOnlyCollection<DAM.Domain.Entities.AllianceEnemy>> GetEnemies(Guid GuildId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetEnemies.{GuildId.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetEnemies(GuildId);

            });
            return results;
        }

        public async Task<DAM.Domain.Entities.AllianceEnemy?> GetEnemy(Guid GuildId, Guid EnemyId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetEnemy.{GuildId.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetEnemy(GuildId, EnemyId);

            });
            return results;
        }

        public async Task<DAM.Domain.Entities.AllianceMember?> GetMember(Guid MemberId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetMember.{MemberId.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetMember(MemberId);

            });
            return results;
        }

        public async Task<DAM.Domain.Entities.AllianceMember?> GetMember(Guid AllianceId, string discordId)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetMember.{AllianceId.ToString()}.{discordId}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetMember(AllianceId, discordId);

            });
            return results;
        }

        public async Task<DAM.Domain.Entities.AllianceMember> GetOrCreateAllianceMember(Guid AllianceId, IGuildUser user)
        {
            var results = await this._cache.GetOrCreateAsync($"AllianceManagementService.GetOrCreateAllianceMember.{AllianceId.ToString()}.{user.Id.ToString()}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _service.GetOrCreateAllianceMember(AllianceId, user);

            });
            return results;
        }

        public Task<DAM.Domain.Entities.Alliance> RegisterAlliance(string discordguildid, string alliancename)
        {
            return _service.RegisterAlliance(discordguildid, alliancename);
        }

        public async Task RemoveMember(Guid GuildId, string discorduserid)
        {
            await _service.RemoveMember(GuildId, discorduserid);
        }

        public async Task RemoveMember(Guid MemberId)
        {
            await _service.RemoveMember(MemberId);
        }

        public async Task<DAM.Domain.Entities.AllianceConfiguration> UpdateAllianceConfiguration(Guid GuildId, DAM.Domain.Entities.AllianceConfiguration allianceConfiguration)
        {
            var alliance = await GetAlliance(GuildId);
            _cache.Remove($"AllianceManagementService.GetAllianceConfiguration.{GuildId.ToString()}");
            _cache.Remove($"AllianceManagementService.GetAlliance.{GuildId.ToString()}");
            _cache.Remove($"AllianceManagementService.GetAlliance.{alliance.DiscordGuildId.ToString()}");

            return await _service.UpdateAllianceConfiguration(GuildId, allianceConfiguration);

        }

        public Task UpdateEnemy(DAM.Domain.Entities.AllianceEnemy enemy)
        {
            return _service.UpdateEnemy(enemy);
        }

        public Task UpdateMember(DAM.Domain.Entities.AllianceMember member)
        {
            return _service.UpdateMember(member);
        }
    }
}
