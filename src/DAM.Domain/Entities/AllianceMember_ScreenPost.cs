using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public class AllianceMember_ScreenPost
    {
        [Key]
        public Guid AllianceMemberId { get; set; }
        [Key]
        public Guid ScreenPostId { get; set; }

        public int? CharacterCount { get; set; }
        public AllianceMember AllianceMember { get; set; } = null!;
        [ForeignKey("ScreenPostId")]
        public ScreenPost ScreenPost { get; set; } = null!;

        [ForeignKey("ScreenPostId")]
        public AtkScreensPost AtkScreensPost { get; set; } = null!;

        [ForeignKey("ScreenPostId")]
        public DefScreensPost DefScreensPost { get; set; } = null!;

    }
}
