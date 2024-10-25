using DAM.Domain.Enums;

namespace DAM.WebApp.Models.Alliance
{
    public class AllianceConfigurationViewModel
    {
        public Guid Id { get; set; }
        public string? AtkScreen_DiscordChannelId { get; set; }
        public string? DefScreen_DiscordChannelId { get; set; }
        public string? Rapport_DiscordChannelId { get; set; }
        public string? DefaultSeasonRankingChannelId { get; set; }
        public string? Ava_DiscordForumChannelId { get; set; }
        public string? ScreenApproverRoleId { get; set; }
        public bool? IsScreenPrepaRequired { get; set; }
        public bool? IsAllianceEnemyRequired { get; set; }

        public bool? AutoValidateNoDef { get; set; }
        public bool? AllowSeasonOverlap { get; set; }
        public BotScreenBehaviorType BotScreenBehaviorType { get; set; }
        public string? BehaviorScreenConfigJSONData { get; set; }
        public Guid? DefaultBaremeAttaqueId { get; set; }
        public Guid? DefaultBaremeDefenceId { get; set; }
    }
}
