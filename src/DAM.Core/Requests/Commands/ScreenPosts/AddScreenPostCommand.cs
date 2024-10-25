using DAM.Core.Abstractions.Requests;
using DAM.Domain.Entities;

namespace DAM.Core.Requests.Commands
{
    public class AddScreenPostCommand : ICommand<Guid>
    {
        public AddScreenPostCommand()
        {
            this.AllianceMembers = new HashSet<ulong>();
        }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePrepUrl { get; set; }
        public ulong DiscordGuildId { get; set; }
        public Guid? AllianceEnemyId { get; set; }
        public int EnemyCount { get; set; }
        public ScreenPostType Type { get; set; }
        public ScreenPostTarget Target { get; set; }

        public ulong? DiscordChannelId { get; set; }
        public ulong? DiscordMessageId { get; set; }

        public ulong? OverridenCreatedByMemberId { get; set; }
        public DateTime? OverridenCreatedon { get; set; }
        public ICollection<ulong> AllianceMembers { get; set; }

    }
}
