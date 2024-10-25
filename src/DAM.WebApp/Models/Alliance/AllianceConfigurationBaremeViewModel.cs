using DAM.Domain.Entities;
using DAM.Domain.Enums;

namespace DAM.WebApp.Models.Alliance
{
    public class AllianceConfigurationBaremeViewModel
    {
        public Guid Id { get; set; }
        public ulong AtkScreen_DiscordChannelId { get; set; }
        public ulong DefScreen_DiscordChannelId { get; set; }
        public ulong? Rapport_DiscordChannelId { get; set; }
        public ulong? Ava_DiscordForumChannelId { get; set; }
        public ulong? DefaultSeasonRankingChannelId { get; set; }
        public ulong ScreenApproverRoleId { get; set; }

        public bool AutoValidateNodef { get; set; }
        public bool AllowSeasonOverlap { get; set; }
        public bool IsScreenPrepaRequired { get; set; }
        public bool IsAllianceEnemyRequired { get; set; }
        public Guid? DefaultDefBareme { get; set; }
        public BotScreenBehaviorType BotScreenBehaviorType { get; set; }
        public string BehaviorScreenConfigJSONData { get; set; }
        public Guid? DefaultAttaqueBareme { get; set; }
        public IEnumerable<Bareme> Baremes { get; set; }
        public IEnumerable<AllianceEnemy> Enemies { get; set; }


    }
}
