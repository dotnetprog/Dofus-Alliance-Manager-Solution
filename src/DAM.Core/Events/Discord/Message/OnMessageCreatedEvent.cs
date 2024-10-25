using Discord.WebSocket;
using MediatR;

namespace DAM.Core.Events.Discord.Message
{
    public class OnMessageCreatedEvent : INotification
    {
        public SocketMessage SocketMessage { get; set; }
    }
}
