using DAM.Core.Abstractions.Requests;

namespace DAM.Core.Requests.Commands
{
    public class CreateSaisonCommand : ICommand<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public Guid AllianceId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool AllowOverlap { get; set; }

        public Guid? BaremeDefenseId { get; set; }
        public Guid? BaremeAttackId { get; set; }

        public ulong DiscordChannelId { get; set; }
    }
}
