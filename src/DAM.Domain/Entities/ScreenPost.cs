using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DAM.Domain.Entities
{

    public enum ScreenPostType { 
    
        Attack,
        Defense
    }
    public enum ScreenPostTarget
    {

        Prisme,
        Perco
    }

    public class ScreenPost
    {
        public ScreenPost()
        {
            this.Members = new HashSet<AllianceMember_ScreenPost>();
           
        }
        [Key]
        public Guid Id { get; set; }
        public string? Description { get; set; }

        public string? Base64Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePrepUrl { get; set; }
        public Guid? AllianceEnemyId { get; set; }
        [ForeignKey("AllianceEnemyId")]
        public AllianceEnemy? AllianceEnemy { get; set; }
        public int? Filesize { get; set; }
        public int EnemyCount { get; set; }
        public ScreenPostType Type { get; set; }
        public ScreenPostTarget Target { get; set; }
        public bool? HasOtherWithSameSize { get; set; }
        public DateTime CreatedOn { get; set; } 

        public ulong? DiscordChannelId { get; set; }
        public ulong? DiscordMessageId { get; set; }

        public Guid? CreatedByMemberId { get; set; }
        [ForeignKey("CreatedByMemberId")]
        public AllianceMember? CreatedBy { get; set; }

        public ICollection<AllianceMember_ScreenPost> Members { get; set; }

        public ICollection<ScreenValidation> ScreenValidations { get; set; }


    }
}
