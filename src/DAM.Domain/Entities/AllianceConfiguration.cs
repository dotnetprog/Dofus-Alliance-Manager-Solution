

using DAM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public class AllianceConfiguration
    {
        public Guid Id { get; set; }
        public ulong AtkScreen_DiscordChannelId { get; set; }
        public ulong DefScreen_DiscordChannelId { get; set; }
        public ulong? Rapport_DiscordChannelId { get; set; }
        public ulong? ScreenApproverRoleId { get; set; }

        public bool? IsScreenPrepaRequired { get; set; }
        public bool? AutoValidateNoDef { get; set; }
        public bool? IsAllianceEnemyRequired { get; set; }
        public bool? AllowSeasonOverlap { get; set; }
        public BotScreenBehaviorType? BotScreenBehaviorType { get; set; }

        public ulong? DefaultSeasonRankingChannelId { get; set; }

        public string? BehaviorScreenConfigJSONData { get; set; }

        public ulong? Ava_DiscordForumChannelId { get; set; }

        public Guid? DefaultBaremeDefenseId { get; set; }
        public Guid? DefaultBaremeAttaqueId { get; set; }
        [ForeignKey("DefaultBaremeDefenseId")]
        public Bareme? DefaultBaremeDefense { get; set; }
        [ForeignKey("DefaultBaremeAttaqueId")]
        public Bareme? DefaultBaremeAttaque { get; set; }

    }
}
