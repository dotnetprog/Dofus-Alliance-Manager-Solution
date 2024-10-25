using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public enum AvAValidationState
    {
        Approved = 1,
        Rejected = 2
    }
    public class AvaMember
    {
        public Guid Id { get; set; }
        
        public Guid AvaId { get; set; }
        [ForeignKey("AvaId")]
        public AvA AvA { get; set; }

        public Guid? MemberId { get; set; }
        [ForeignKey("MemberId")]
        public AllianceMember? Member { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ImageUrl { get; set; }
        public AvAValidationState? ValidationState { get; set; }

        public Guid? ValidatedById { get; set; }
        [ForeignKey("ValidatedById")]
        public AllianceMember? ValidatedBy { get; set; }

        public int? MontantPepites { get; set; }
        public ulong DiscordMessageId { get; set; }

    }
}
