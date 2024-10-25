using DAM.Domain.Entities;

namespace DAM.WebApp.Models.Api.ScreenPosts
{
    public class CreateScreenPostRequest
    {
        public CreateScreenPostRequest()
        {
            this.DiscordUserIds = new HashSet<string>();
        }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public string? ImagePrepUrl { get; set; }
        public Guid? AllianceEnemyId { get; set; }
        public int EnemyCount { get; set; }
        public ScreenPostType Type { get; set; }
        public ScreenPostTarget Target { get; set; }

        public string DiscordChannelId { get; set; }
        public string DiscordMessageId { get; set; }

        public string CreatedByDiscordUserId { get; set; }
        public DateTime? OverridenCreatedon { get; set; }
        public ICollection<string> DiscordUserIds { get; set; }

    }
}
