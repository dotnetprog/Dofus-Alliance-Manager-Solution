using DAM.Core.Helpers.Utility;
using DAM.Domain.Entities;
using Discord;
using System.Globalization;

namespace DAM.Core.Helpers.Discord
{

    public interface IRankingRowFormat
    {
        string Format(SaisonRanking ranking, int index);
    }

    public class TotalPepiteRankingFowFormat : IRankingRowFormat
    {
        private static string[] podium = new string[] { "🏆", "🥈", "🥉" };
        private static Emote PepiteEmoji = Emote.Parse("<:pepite:1095170378013360291>");
        public string Format(SaisonRanking ranking, int index)
        {
            var rankEmote = podium.Length < ranking.PositionRank ? $"#{ranking.PositionRank.ToString("D3")}" : podium[ranking.PositionRank - 1];

            return $"{rankEmote} **{ranking.Member.Nickname ?? ranking.Member.Alias}** : {ranking.MontantTotalPepite} points {Environment.NewLine}";
        }


    }

    public class EmbedHelper
    {
        public const int EMBED_DESCRIPTION_MAXSIZE = 4096;
        public const int EMBED_FIELD_MAXSIZE = 256;

        public static Emote PepiteEmoji = Emote.Parse("<:pepite:1095170378013360291>");






        public static Embed[] BuildPlayerList(string name, string iconurl, string headerTitle, IReadOnlyCollection<IGuildUser> members)
        {
            var embeds = new List<Embed>();
            var headerEmbedBuilder = new EmbedBuilder()
              .WithAuthor(name, iconurl)
              .WithTitle(headerTitle)
            .WithColor(Color.DarkBlue);
            var stringRankings = members.Select(m => $"{m.Mention}\n");
            var totalslices = Math.Ceiling((((decimal)stringRankings.Sum(s => s.Length)) / ((decimal)EMBED_DESCRIPTION_MAXSIZE)));

            var currentindex = 1;
            var currentEmbed = new EmbedBuilder();
            currentEmbed.WithAuthor(name, iconurl).WithTitle($"Liste ({currentindex}/{totalslices})");


            foreach (var ranking in stringRankings)
            {

                if (((currentEmbed.Description?.Length ?? 0) + ranking.Length) > EMBED_DESCRIPTION_MAXSIZE)
                {
                    embeds.Add(currentEmbed.Build());
                    currentindex++;
                    currentEmbed = new EmbedBuilder().WithTitle($"Liste ({currentindex}/{totalslices})"); ;
                }
                currentEmbed.Description += ranking;
            }
            embeds.Add(currentEmbed.Build());// add the last embed

            return embeds.ToArray();
        }

        public static Embed[] BuildLadderEmbed(string name, string iconurl, Saison saison, IReadOnlyCollection<SaisonRanking> rankings, IRankingRowFormat formatter = null, bool useOrderByPositionRank = true)
        {
            var title = $"{saison.Name}";
            var startDate = $"{saison.StartDate.ToString("f", CultureInfo.CreateSpecificCulture("fr-FR"))} (UTC)";
            var endDate = $"{saison.EndDate.ToString("f", CultureInfo.CreateSpecificCulture("fr-FR"))} (UTC)";
            var ladderGeneratedOn = string.Empty;
            if (saison.LadderGeneratedOn.HasValue)
            {
                var ladderGeneratedOnRaw = saison.LadderGeneratedOn.Value.ToParisTime();
                ladderGeneratedOn = $"{ladderGeneratedOnRaw.ToString("f", CultureInfo.CreateSpecificCulture("fr-FR"))} (Paris)";
            }

            if (formatter == null)
            {
                formatter = new TotalPepiteRankingFowFormat();
            }

            var stringRankings = useOrderByPositionRank ? rankings.OrderBy(r => r.PositionRank).Select(formatter.Format).ToArray() : rankings.Select(formatter.Format).ToArray();


            var ladderEmbeds = new List<Embed>();


            var headerEmbedBuilder = new EmbedBuilder()
                .WithAuthor(name, iconurl)
                .WithTitle(title)
                .WithColor(Color.DarkBlue)
                .AddField("Début de la saison", startDate, true)
                .AddField("Fin de la saison", endDate, true);
            if (!string.IsNullOrWhiteSpace(ladderGeneratedOn))
            {
                headerEmbedBuilder.AddField("Classement rafraîchit le", ladderGeneratedOn);
            }

            ladderEmbeds.Add(headerEmbedBuilder.Build());


            var totalslices = Math.Ceiling((((decimal)stringRankings.Sum(s => s.Length)) / ((decimal)EMBED_DESCRIPTION_MAXSIZE)));

            var currentindex = 1;
            var currentEmbed = new EmbedBuilder();
            currentEmbed.WithAuthor(name, iconurl).WithTitle($"Classement ({currentindex}/{totalslices})");


            foreach (var ranking in stringRankings)
            {

                if (((currentEmbed.Description?.Length ?? 0) + ranking.Length) > EMBED_DESCRIPTION_MAXSIZE)
                {
                    ladderEmbeds.Add(currentEmbed.Build());
                    currentindex++;
                    currentEmbed = new EmbedBuilder().WithTitle($"Classement ({currentindex}/{totalslices})"); ;
                }
                currentEmbed.Description += ranking;
            }
            ladderEmbeds.Add(currentEmbed.Build());// add the last embed

            return ladderEmbeds.ToArray();
        }




    }
}
