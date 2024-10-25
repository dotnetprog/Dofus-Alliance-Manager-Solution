using DAM.Core.Helpers.Discord;
using DAM.WebApp.Models.Alliance;
using Discord;
using System.Globalization;

namespace DAM.WebApp.Common.Helper.Discord
{
    public static class WebAppEmbedHelper
    {

        public static Embed[] BuildReportEmbed(string authtorName, string authorIconUrl, DateTime Start, DateTime End, SummaryReport report, IReportRowFormat formatter)
        {
            var startDate = $"{Start.ToString("f", CultureInfo.CreateSpecificCulture("fr-FR"))}";
            var endDate = $"{End.ToString("f", CultureInfo.CreateSpecificCulture("fr-FR"))}";

            var summaryEmbed = new EmbedBuilder()
                .WithAuthor(authtorName, authorIconUrl)
                .WithTitle($"Sommaire du rapport {EmbedHelper.PepiteEmoji}")
                .AddField($"Moyenne par joueur", string.Format("{0:0.00}", report.MoyennePepiteParJoueur), true)
                .AddField($"Coût total pour l'alliance", string.Format("{0:0.00}", report.TotalPepites), true)
                .AddField("Début de la période", startDate)
                .AddField("Fin de la période", endDate)
                .WithColor(Color.Orange).Build();


            var stringRankings = report.data.Select(formatter.Format).ToArray();
            var totalslices = Math.Ceiling((((decimal)stringRankings.Sum(s => s.Length)) / ((decimal)EmbedHelper.EMBED_DESCRIPTION_MAXSIZE)));
            var currentindex = 1;
            var currentEmbed = new EmbedBuilder();
            currentEmbed.WithAuthor(authtorName, authorIconUrl)
                .WithTitle($"Rapport {EmbedHelper.PepiteEmoji} ({currentindex}/{totalslices})");

            var rapportEmbeds = new List<Embed>();
            rapportEmbeds.Add(summaryEmbed);

            foreach (var ranking in stringRankings)
            {

                if (((currentEmbed.Description?.Length ?? 0) + ranking.Length) > EmbedHelper.EMBED_DESCRIPTION_MAXSIZE)
                {
                    rapportEmbeds.Add(currentEmbed.Build());
                    currentindex++;
                    currentEmbed = new EmbedBuilder().WithTitle($"Rapport {EmbedHelper.PepiteEmoji} ({currentindex}/{totalslices})"); ;
                }
                currentEmbed.Description += ranking;
            }
            rapportEmbeds.Add(currentEmbed.Build());

            return rapportEmbeds.ToArray();




        }
    }
}
