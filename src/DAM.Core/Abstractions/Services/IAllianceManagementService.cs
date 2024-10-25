
using DAM.Domain.Entities;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Core.Abstractions.Services
{
    public interface IAllianceManagementServiceAsync
    {
        Task<AllianceMember> GetOrCreateAllianceMember(Guid AllianceId, IGuildUser user);
        Task<Alliance> RegisterAlliance(string discordguildid, string alliancename);

        Task<AllianceConfiguration> UpdateAllianceConfiguration(Guid GuildId, AllianceConfiguration allianceConfiguration);

        Task<Guid> AddMember(AllianceMember member);
        Task UpdateMember(AllianceMember member);
        Task RemoveMember(Guid GuildId,string discorduserid);
        Task RemoveMember(Guid MemberId);


        Task<IReadOnlyCollection<AllianceMember>> GetAllMembers(Guid AllianceId);

        Task<AllianceMember?> GetMember(Guid MemberId);
        Task<AllianceMember?> GetMember(Guid AllianceId,string discordId);

        Task<AllianceConfiguration?> GetAllianceConfiguration(Guid GuildId);

        Task<Alliance?> GetAlliance(Guid AllianceId); 
        Task<Alliance?> GetAlliance(string DiscordGuildId);

        Task<IReadOnlyCollection<AllianceEnemy>> GetEnemies(Guid GuildId);
        Task<AllianceEnemy?> GetEnemy(Guid GuildId, Guid EnemyId);
        Task<Guid> AddEnemy(AllianceEnemy enemy);
        Task UpdateEnemy(AllianceEnemy enemy);
        Task DeleteEnemy(Guid GuildId,Guid EnemyId);

    }
}
