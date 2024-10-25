namespace DAM.Domain.Exceptions
{
    public class InvalidDiscordUserException : Exception
    {
        public ulong DiscordUserId { get; set; }

        public ulong DiscordGuildId { get; set; }

        public InvalidDiscordUserException(ulong discordUserId, ulong discordGuildId) : base($"Discord User {discordUserId} is not part of guild {discordGuildId}")
        {
            DiscordUserId = discordUserId;
            DiscordGuildId = discordGuildId;
        }
    }
}
