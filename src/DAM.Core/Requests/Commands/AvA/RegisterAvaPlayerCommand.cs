using MediatR;

namespace DAM.Core.Requests.Commands.AvA
{
    public class RegisterAvaPlayerCommand : IRequest<Guid>
    {
        public Guid AllianceId { get; set; }
        public Guid AvAId { get; set; }
        public ulong PlayerDiscordId { get; set; }
        public ulong DiscordMessageId { get; set; }
        public string ScreenAttachmentUrl { get; set; }

    }
}
