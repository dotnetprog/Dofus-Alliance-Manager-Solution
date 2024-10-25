using DAM.Domain.Entities;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Core.Abstractions.Services
{
    public interface IAvAService
    {
        Task<Guid> CreateAvA(AvA ava);
        Task UpdateAva(AvA ava);
        Task DeleteAvA(Guid AllianceId, Guid Id);

        Task<AvA?> GetAvA(Guid AllianceId, Guid Id,bool includeMembers = false);
        Task<AvA?> GetAvA(Guid AllianceId, ulong discordChannelId);
        Task<IReadOnlyCollection<AvA>> GetAvAList(Guid AllianceId);

        Task<Guid> RegisterAvAMember(AvaMember member);
        Task EditAvAMember(AvaMember member);
        Task RemoveAvAMember(Guid memberid);
        Task PayMembers(Guid Id,int MontantPepites);
        Task<AvaMember?> GetMember(Guid memberid);
        Task<AvaMember?> GetMember(Guid AvaId,Guid AllianceMemberId);
        Task<AvaMember?> GetMember(Guid AvaId, ulong DiscordMessageId);
        Task<IReadOnlyCollection<AvaMember>> GetMembers(Guid AvaId);

    }
}
