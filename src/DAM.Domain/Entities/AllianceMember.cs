
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DAM.Domain.Entities
{
    public enum MemberState
    {
        Active = 0,
        Inactive = 1
    }
    public enum MemberStatus
    {
        Active = 0,
        Inactive = 1,
        Removed = 2,
        Banned = 3
    }
    public class AllianceMember
    {
        public AllianceMember()
        {
            this.ScreenPosts = new HashSet<AllianceMember_ScreenPost>();
            this.State = MemberState.Active;
            this.Status = MemberStatus.Active;
        }
        [Key]
        public Guid Id { get; set; }

        public Guid? AllianceId { get; set; }
        [ForeignKey("AllianceId")]
        public Alliance? Alliance { get; set; }
        public string? DiscordId { get; set; }
        public string? Alias { get; set; }
        public string? Nickname { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public MemberState State { get; set; }
        public MemberStatus Status { get; set; }
        public ICollection<AllianceMember_ScreenPost> ScreenPosts { get; set; }

        public ICollection<AnkamaPseudo> AnkamaPseudos { get; set; }


        public bool? DonotAlert { get; set; }

    }
}
