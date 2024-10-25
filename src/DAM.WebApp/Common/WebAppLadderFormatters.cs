using DAM.WebApp.Models.Alliance;
using Discord;

namespace DAM.WebApp.Common
{
    public interface IReportRowFormat
    {
        string Format(SummaryReportRowViewModel ranking, int index);
    }
    public class WebAppLadderFormatters : IReportRowFormat
    {
        private static string[] podium = new string[] { "🏆", "🥈", "🥉" };
        private static Emote PepiteEmoji = Emote.Parse("<:pepite:1095170378013360291>");
        public string Format(SummaryReportRowViewModel row, int index)
        {
            var rankEmote = (podium.Length - 1) < index ? $"#{(index + 1).ToString("D3")}" : podium[index];

            return $"{rankEmote} {row.Username}({row.DiscordId}) : {string.Format("{0:0.00}", row.MontantTotalPepites)} {PepiteEmoji.ToString()} {Environment.NewLine}";
        }
    }
}
