
using DAM.Core.Abstractions.Services;
using DAM.Core.Helpers.Discord;
using DAM.Core.Helpers.LadderFormatter;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;
using Discord;


namespace DAM.Core.Requests.Commands.Saisons
{
    public class PublishSaisonRankingsCommandHandler : BaseSimpleCommandHandler<PublishSaisonRankingsCommand>
    {
        private readonly IDiscordBotService _botService;
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly ISaisonServiceAsync _saisonService;
        private readonly IWebAppMetadataService _webappMetadata;
        public PublishSaisonRankingsCommandHandler(IDiscordBotService botService,
            IAllianceManagementServiceAsync allianceManagementService,
            ISaisonServiceAsync saisonService,
            IWebAppMetadataService webAppMetadata)
        {
            _botService = botService;
            _allianceManagementService = allianceManagementService;
            _saisonService = saisonService;
            _webappMetadata = webAppMetadata;
        }

        public override async Task Run(PublishSaisonRankingsCommand request, CancellationToken cancellationToken)
        {

            var alliance = await _allianceManagementService.GetAlliance(request.GuidServerId.ToString());
            if (!(alliance?.AllianceConfiguration?.Rapport_DiscordChannelId.HasValue ?? false))
            {
                throw new InvalidEntityOperationException(request.GuidServerId.ToString(), "Alliance", EntityOperation.Retrieve, $"{nameof(AllianceConfiguration.Rapport_DiscordChannelId)} is missing");
            }
            var season = await _saisonService.GetById(alliance.Id, request.SaisonId);

            var rankings = await _saisonService.GetRankings(alliance.Id, season.Id, true);

            var guild = await _botService.GetGuild(request.GuidServerId);
            if (guild == null)
            {
                throw new ArgumentException(nameof(PublishSaisonRankingsCommand.GuidServerId), "Server do not exist.");
            }
            var targetChannelId = season.SeasonRankingChannelId ?? alliance.AllianceConfiguration.Rapport_DiscordChannelId.Value;
            var targetChannel = (ITextChannel)(await guild.GetChannelAsync(targetChannelId));
            if (!rankings.Any())
            {
                return;
            }
            if (request.ClearChannel)
            {
                var messages = await targetChannel.GetMessagesAsync().FlattenAsync();
                if (messages.Any())
                {
                    await targetChannel.DeleteMessagesAsync(messages);
                }
            }



            var embeds = EmbedHelper.BuildLadderEmbed(guild.Name, guild.IconUrl ?? string.Empty, season, rankings, new WebAppRowLadderFormatter(_webappMetadata, guild.Id));

            foreach (var embed in embeds)
            {
                await targetChannel.SendMessageAsync(embed: embed);
            }
        }
    }
}
