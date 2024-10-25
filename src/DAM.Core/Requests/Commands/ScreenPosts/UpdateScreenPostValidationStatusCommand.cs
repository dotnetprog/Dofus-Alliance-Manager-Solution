using DAM.Core.Abstractions.Requests;
using DAM.Domain.Entities;
using MediatR;

namespace DAM.Core.Requests.Commands.ScreenPosts
{
    public class UpdateScreenPostValidationStatusCommand : ICommand<Unit>
    {
        public string Screenkey { get; set; }
        public ulong DiscordGuildId { get; set; }

        public ScreenValidationResultStatus Status { get; set; }

        public string ClosedByDiscordUserId { get; set; }



    }
}
