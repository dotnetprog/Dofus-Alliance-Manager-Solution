using DAM.Core.Abstractions.Requests;
using MediatR;

namespace DAM.WebApp.Requests.Commands.Reports
{
    public class PublishReportCommand : ICommand<Unit>
    {
        public Guid AllianceId { get; set; }
        public ulong DiscordChannelId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal Multipler { get; set; }
        public Guid BaremeAttaqueId { get; set; }
        public Guid BaremeDefenseId { get; set; }
    }
}
