using MediatR;

namespace DAM.Core.Events.Discord.Message
{
    public class OnMessageDeletedEvent : INotification
    {
        public ulong ChannelId { get; set; }
        public ulong DiscordMessageId { get; set; }
    }
}
