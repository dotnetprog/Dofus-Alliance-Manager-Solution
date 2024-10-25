namespace DAM.WebApp.Models.Api.ScreenPosts
{
    public class UpdateScreenPostRequest
    {
        public UpdateScreenPostRequest()
        {
            this.DiscordUserIds = new HashSet<string>();
        }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePrepUrl { get; set; }
        public Guid? AllianceEnemyId { get; set; }
        public ICollection<string> DiscordUserIds { get; set; }
    }
}
