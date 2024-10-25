using DAM.Core.Abstractions.Services;
using DAM.Core.Helpers.Discord;
using DAM.Domain.Entities;
using Discord;

namespace DAM.Core.Helpers.LadderFormatter
{
    public class WebAppRowLadderFormatter : IRankingRowFormat
    {
        private static string[] podium = new string[] { "🏆", "🥈", "🥉" };
        private static Emote PepiteEmoji = Emote.Parse("<:pepite:1095170378013360291>");
        private static Emote GreenArrowEmoji = Emote.Parse("<:5720greenarrowup:1218955916930711633>");
        private static Emote RedArrowEmoji = Emote.Parse("<:2456redarrowdown:1218955918146801675>");
        private readonly IWebAppMetadataService _metadataService;
        private readonly ulong discordserverid;
        public WebAppRowLadderFormatter(IWebAppMetadataService metadataService, ulong dsServerId)
        {
            _metadataService = metadataService;
            this.discordserverid = dsServerId;
        }

        public string Format(SaisonRanking ranking, int index)
        {
            var rankEmote = podium.Length < ranking.PositionRank ? $"#{ranking.PositionRank.ToString("D3")}" : podium[ranking.PositionRank - 1];

            var diffPosition = ranking.Previous_PositionRank - ranking.PositionRank;

            var diffString = $"";
            if (diffPosition.HasValue && diffPosition != 0)
            {
                diffString = diffPosition < 0 ? $"{RedArrowEmoji}**{diffPosition}**" : $"{GreenArrowEmoji}**+{diffPosition}**";
            }


            var playerLink = _metadataService.GetSaisonPlayerUrl(discordserverid, ranking.SaisonId, ranking.Member.DiscordId);
            var Playertag = $"[{ranking.Member.Nickname ?? ranking.Member.Alias}](<{playerLink}>)";
            return $"{rankEmote} **{Playertag}** : {ranking.MontantTotalPepite} points {diffString}{Environment.NewLine}";
        }
    }
}
