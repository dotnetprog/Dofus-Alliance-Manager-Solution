using DAM.Core.Abstractions.Requests;
using MediatR;

namespace DAM.Core.Requests.Commands.ScreenPosts
{
    public class UpdateScreenPostCommand : ICommand<Unit>
    {
        public UpdateScreenPostCommand()
        {
            this.AllianceMembers = new HashSet<ulong>();
        }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePrepUrl { get; set; }
        public ulong DiscordGuildId { get; set; }
        public ulong DiscordMessageId { get; set; }
        public Guid? AllianceEnemyId { get; set; }

        public ICollection<ulong> AllianceMembers { get; set; }


    }
}
