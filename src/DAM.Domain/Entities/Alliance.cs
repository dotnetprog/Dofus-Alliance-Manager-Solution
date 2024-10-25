
using System.ComponentModel.DataAnnotations.Schema;


namespace DAM.Domain.Entities
{
    public class Alliance
    {
        public Alliance() {

            this.Members = new HashSet<AllianceMember>();
        }
        public Guid Id { get; set; }

        public string? Alias { get; set; }
        public string? DiscordGuildId { get; set; }

        public DateTime? RegisteredOn { get; set; }
        public Guid? AllianceConfigurationId { get; set; }
        [ForeignKey("AllianceConfigurationId")]
        public AllianceConfiguration? AllianceConfiguration { get; set; }

        public ICollection<AllianceMember> Members { get; set; }

    }
}
