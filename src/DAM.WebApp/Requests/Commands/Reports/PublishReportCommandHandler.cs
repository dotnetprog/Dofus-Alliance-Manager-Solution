using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands;
using DAM.WebApp.Common;
using DAM.WebApp.Common.Helper.Discord;
using DAM.WebApp.Models.Alliance;
using Discord;

namespace DAM.WebApp.Requests.Commands.Reports
{
    public class PublishReportCommandHandler : BaseSimpleCommandHandler<PublishReportCommand>
    {
        private readonly IDiscordBotService _botService;
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IReportServiceAsync _reportServiceAsync;
        public PublishReportCommandHandler(IDiscordBotService botService, IAllianceManagementServiceAsync allianceManagementService, IReportServiceAsync reportServiceAsync)
        {
            _botService = botService;
            _allianceManagementService = allianceManagementService;
            _reportServiceAsync = reportServiceAsync;
        }
        public override async Task Run(PublishReportCommand request, CancellationToken cancellationToken)
        {
            var alliance = await _allianceManagementService.GetAlliance(request.AllianceId);
            var rows = await _reportServiceAsync.GetReportData(request.AllianceId,
                request.BaremeAttaqueId, request.BaremeDefenseId, request.From.ToUniversalTime(), request.To.ToUniversalTime());
            if (!rows.Any())
            {
                return;
            }
            var targetchannel = (ITextChannel)(await _botService.GetChannel(ulong.Parse(alliance.DiscordGuildId), request.DiscordChannelId));
            var guild = targetchannel.Guild;
            var multiplier = request.Multipler;

            var vmRows = rows.Select(r => new SummaryReportRowViewModel(r)).ToList();
            var report = new SummaryReport
            {
                TotalPepitesAtk = vmRows.Sum(r => r.MontantAtkPepites) * multiplier,
                TotalPepitesDef = vmRows.Sum(r => r.MontantDefPepites) * multiplier,
                TotalPepitesAvA = vmRows.Sum(r => r.MontantAvAPepites) * multiplier,
                TotalPepites = vmRows.Sum(r => r.MontantTotalPepites) * multiplier,
                MoyennePepiteParJoueur = vmRows.Any() ? vmRows.Average(r => r.MontantTotalPepites) * multiplier : 0,
                data = vmRows.OrderByDescending(r => r.MontantTotalPepites).Select(c =>
                {
                    c.MontantDefPepites = c.MontantDefPepites * multiplier;
                    c.MontantAvAPepites = c.MontantAvAPepites * multiplier;
                    c.MontantAtkPepites = c.MontantAtkPepites * multiplier;
                    c.MontantTotalPepites = c.MontantTotalPepites * multiplier;
                    return c;

                }),
            };


            var embeds = WebAppEmbedHelper.BuildReportEmbed(guild.Name,
                guild.IconUrl ?? string.Empty,
                request.From, request.To,
                report,
                new WebAppLadderFormatters());

            foreach (var embed in embeds)
            {
                await targetchannel.SendMessageAsync(embed: embed);
            }

        }
    }
}
