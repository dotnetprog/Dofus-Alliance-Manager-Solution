using Discord;
using MediatR;

namespace DAM.Core.Events.Discord.Message
{
    public class OnMessageUpdatedEvent : INotification
    {
        public IMessage Message { get; set; }
    }
}
