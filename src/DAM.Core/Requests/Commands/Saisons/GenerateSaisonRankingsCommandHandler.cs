using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;

namespace DAM.Core.Requests.Commands.Saisons
{
    public class GenerateSaisonRankingsCommandHandler : BaseSimpleCommandHandler<GenerateSaisonRankingsCommand>
    {
        private readonly ISaisonServiceAsync _saisonServiceAsync;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IDiscordBotService _botService;
        private readonly IReportServiceAsync _reportServiceAsync;
        private readonly IDAMMapper _mapper;


        public GenerateSaisonRankingsCommandHandler(IAllianceManagementServiceAsync allianceManagementServiceAsync,
            ISaisonServiceAsync saisonServiceAsync,
            IReportServiceAsync reportServiceAsync,
            IDAMMapper mapper,
            IDiscordBotService botService)
        {
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _saisonServiceAsync = saisonServiceAsync;
            _reportServiceAsync = reportServiceAsync;
            _mapper = mapper;
            _botService = botService;
        }

        private async Task<AllianceMember> GetOrCreateMember(Guid AllianceId, ulong GuildId, ulong discordid)
        {
            var discorduser = await _botService.GetGuildUser(GuildId, discordid);
            if (discorduser == null)
            {
                throw new InvalidDiscordUserException(discordid, GuildId);
            }
            var member = await this._allianceManagementServiceAsync.GetOrCreateAllianceMember(AllianceId, discorduser);
            return member;
        }
        public override async Task Run(GenerateSaisonRankingsCommand request, CancellationToken cancellationToken)
        {
            var saison = await this._saisonServiceAsync.GetById(request.AllianceId, request.SaisonId);
            if (saison == null)
            {
                throw new RecordNotFoundException(request.SaisonId, nameof(Saison));
            }
            var alliance = await this._allianceManagementServiceAsync.GetAlliance(request.AllianceId);


            var currentUser = await GetOrCreateMember(request.AllianceId, ulong.Parse(alliance.DiscordGuildId), request.CreatedByUserId);


            var allianceConfig = alliance.AllianceConfiguration;
            var currentRankings = await this._saisonServiceAsync.GetRankings(request.AllianceId, request.SaisonId);


            var report = await _reportServiceAsync.GetReportData(request.AllianceId,
                 saison.BaremeAttackId ?? allianceConfig.DefaultBaremeAttaqueId.Value,
                 saison.BaremeDefenseId ?? allianceConfig.DefaultBaremeDefenseId.Value,
                 saison.StartDate,
                 saison.EndDate);
            var rankings = report.Select(r => MapRanking(currentUser, r, currentRankings, saison))
                                 .OrderByDescending(d => d.MontantTotalPepite)
                                 .Select((sr, index) =>
                                 {
                                     sr.PositionRank = index + 1;
                                     return sr;
                                 }).ToArray();
            if (currentRankings.Any())
            {

                await _saisonServiceAsync.ClearRankings(saison.Id);
            }

            await _saisonServiceAsync.CreateRankings(rankings);
            if (!saison.LadderGeneratedOn.HasValue ||
                saison.LadderGeneratedOn.Value.Date < DateTime.UtcNow.Date)//Only set the previous values when date changes
            {
                saison.LadderGeneratedOn = DateTime.UtcNow;
                await _saisonServiceAsync.Update(saison);
            }


        }
        private SaisonRanking MapRanking(AllianceMember CurrentUser, SummaryReportRow reportrow, IReadOnlyCollection<SaisonRanking> rankings, Saison currentSeason)
        {
            var new_ranking_row = this._mapper.Map<SaisonRanking, SummaryReportRow>(reportrow);

            new_ranking_row.SaisonId = currentSeason.Id;
            new_ranking_row.CreatedOn = DateTime.UtcNow;
            new_ranking_row.ModifiedOn = DateTime.UtcNow;
            new_ranking_row.CreatedById = CurrentUser.Id;
            new_ranking_row.ModifiedById = CurrentUser.Id;
            var previous_ranking = rankings.FirstOrDefault(r => r.MemberId == new_ranking_row.MemberId);


            if (previous_ranking != null)
            {
                new_ranking_row.BonusPepite = previous_ranking?.BonusPepite;
                new_ranking_row.bonusReason = previous_ranking?.bonusReason;
                if (currentSeason.LadderGeneratedOn.HasValue &&
                    currentSeason.LadderGeneratedOn.Value.Date < DateTime.UtcNow.Date)//Only set the previous values when date changes
                {
                    new_ranking_row.Previous_PositionRank = previous_ranking.PositionRank;
                    new_ranking_row.Previous_BonusPepite = previous_ranking.BonusPepite;
                    new_ranking_row.Previous_bonusReason = previous_ranking.bonusReason;

                    new_ranking_row.Previous_Nombre_defense = previous_ranking.Nombre_defense;
                    new_ranking_row.Previous_Nombre_attaques = previous_ranking.Nombre_attaques;
                    new_ranking_row.Previous_NombreParticipationAvA = previous_ranking.NombreParticipationAvA;

                    new_ranking_row.Previous_MontantDefPepites = previous_ranking.MontantDefPepites;
                    new_ranking_row.Previous_MontantAtkPepites = previous_ranking.MontantAtkPepites;
                    new_ranking_row.Previous_MontantAvAPepites = previous_ranking.MontantAvAPepites;
                    new_ranking_row.Previous_MontantTotalPepite = previous_ranking.MontantTotalPepite;
                }
                new_ranking_row.CreatedById = previous_ranking.CreatedById;
                new_ranking_row.CreatedOn = previous_ranking.CreatedOn;
            }


            //if (DiscordUsersConsts.DepouyeIds.Contains(long.Parse(reportrow.DiscordId)))
            //{
            //    new_ranking_row.NombreParticipationAvA = 0;
            //    new_ranking_row.Nombre_attaques = 0;
            //    new_ranking_row.Nombre_defense = 0;
            //    new_ranking_row.BonusPepite = 0;
            //    new_ranking_row.MontantAtkPepites = 0;
            //    new_ranking_row.MontantDefPepites = 0;
            //    new_ranking_row.MontantAvAPepites = 0;
            //}

            new_ranking_row.MontantTotalPepite = calculate_total_points(new_ranking_row);
            return new_ranking_row;

        }
        private int calculate_total_points(SaisonRanking ranking) => ranking.MontantAtkPepites + ranking.MontantAvAPepites + ranking.MontantDefPepites + (ranking.BonusPepite ?? 0);
    }
}
