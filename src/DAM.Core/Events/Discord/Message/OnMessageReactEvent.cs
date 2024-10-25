using Discord;
using Discord.WebSocket;
using MediatR;

namespace DAM.Core.Events.Discord.Message
{
    public class OnMessageReactEvent : INotification
    {
        public IMessage Message { get; set; }
        public SocketReaction Reaction { get; set; }

    }
}
