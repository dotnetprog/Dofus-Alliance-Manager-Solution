using DAM.Core.Abstractions.Requests;
using MediatR;

namespace DAM.Core.Requests.Commands
{
    public class UpdateSaisonCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public Guid AllianceId { get; set; }
        public ulong ModifiedByDiscordId { get; set; }
        public ulong? DiscordChannelId { get; set; }
        public bool AllowOverlap { get; set; }
        public Guid? BaremeDefenseId { get; set; }
        public Guid? BaremeAttackId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
