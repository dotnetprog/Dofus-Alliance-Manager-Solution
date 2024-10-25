using DAM.Core.Requests.Commands;
using DAM.Core.Requests.Commands.Saisons;
using DAM.Domain.Entities;
using Mapster;

namespace DAM.Core.Mappings.Configs
{
    public class SaisonsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<CreateSaisonCommand, Saison>()
                .Map(d => d.Description, s => s.Description)
                 .Map(d => d.AllianceId, s => s.AllianceId)
                 .Map(d => d.CreatedById, s => s.CreatedByUserId)
                 .Map(d => d.CreatedOn, s => DateTime.UtcNow)
                 .Map(d => d.EndDate, s => s.EndDate)
                 .Map(d => d.ModifiedById, s => s.CreatedByUserId)
                 .Map(d => d.ModifiedOn, s => DateTime.UtcNow)
                 .Map(d => d.Name, s => s.Name)
                 .Map(d => d.BaremeAttackId, s => s.BaremeAttackId)
                 .Map(d => d.BaremeDefenseId, s => s.BaremeDefenseId)
                 .Map(d => d.SeasonRankingChannelId, s => s.DiscordChannelId)
                 .Map(d => d.StartDate, s => s.StartDate);
            config.ForType<UpdateSaisonCommand, Saison>()
                .Map(d => d.Description, s => s.Description)
                 .Map(d => d.AllianceId, s => s.AllianceId)
                 .Map(d => d.ModifiedOn, s => DateTime.UtcNow)
                 .Map(d => d.EndDate, s => s.EndDate)
                 .Map(d => d.Name, s => s.Name)
                 .Map(d => d.StartDate, s => s.StartDate)
                 .Map(d => d.SeasonRankingChannelId, s => s.DiscordChannelId)
                 .Map(d => d.BaremeAttackId, s => s.BaremeAttackId)
                 .Map(d => d.BaremeDefenseId, s => s.BaremeDefenseId)
                 .Map(d => d.Id, s => s.Id);
            config.ForType<Saison, UpdateSaisonCommand>()
                .Map(d => d.Description, s => s.Description)
                 .Map(d => d.AllianceId, s => s.AllianceId)
                 .Map(d => d.EndDate, s => s.EndDate.ToUniversalTime())
                 .Map(d => d.Name, s => s.Name)
                 .Map(d => d.BaremeAttackId, s => s.BaremeAttackId)
                 .Map(d => d.BaremeDefenseId, s => s.BaremeDefenseId)
                 .Map(d => d.StartDate, s => s.StartDate.ToUniversalTime())
                 .Map(d => d.DiscordChannelId, s => s.SeasonRankingChannelId)
                 .Map(d => d.Id, s => s.Id);
            config.ForType<SummaryReportRow, SaisonRanking>()
                .Map(d => d.MemberId, s => s.MemberId)
                .Map(d => d.Nombre_attaques, s => s.Nombre_attaques)
                .Map(d => d.Nombre_defense, s => s.Nombre_defense)
                .Map(d => d.NombreParticipationAvA, s => s.NombreParticipationAvA)
                .Map(d => d.MontantDefPepites, s => s.MontantDefPepites)
                .Map(d => d.MontantAtkPepites, s => s.MontantAtkPepites)
                .Map(d => d.MontantAvAPepites, s => s.MontantAvAPepites);

            config.ForType<SaisonRanking, UpdateSaisonRankingCommand>()
                .Map(d => d.SaisonRankingId, s => s.Id)
                .Map(d => d.BonusReason, s => s.bonusReason)
                .Map(d => d.Bonus, s => s.BonusPepite);

        }
    }
}
