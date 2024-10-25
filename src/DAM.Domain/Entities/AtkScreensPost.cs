using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public class AtkScreensPost
    {
        public AtkScreensPost()
        {

            this.Members = new HashSet<AllianceMember_ScreenPost>();
        }
        public Guid Id { get; set; }
        public Guid? AllianceId { get; set; }
        public string? Description { get; set; }

        public ScreenPostTarget? Target { get; set; }
        public Guid? CreatedByMemberId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePrepUrl { get; set; }
        public Guid? AllianceEnemyId { get; set; }
        [ForeignKey("AllianceEnemyId")]
        public AllianceEnemy? AllianceEnemy { get; set; }
        public int? Nombre_Allie { get; set; }
        public int? Nombre_Enemi { get; set; }

        public ulong? DiscordChannelId { get; set; }
        public ulong? DiscordMessageId { get; set; }
        public ScreenValidationStatus? StatutTraitementValidation { get; set; }
        public ScreenValidationResultStatus? StatutResultatValidation { get; set; }
        [ForeignKey("CreatedByMemberId")]
        public AllianceMember? CreatedBy { get; set; }

        public Guid? ClosedByMemberId { get; set; }
        [ForeignKey("ClosedByMemberId")]
        public AllianceMember? ClosedBy { get; set; }
        public DateTime? ClosedOn { get; set; }

        public ICollection<AllianceMember_ScreenPost> Members { get; set; }

    }
}
