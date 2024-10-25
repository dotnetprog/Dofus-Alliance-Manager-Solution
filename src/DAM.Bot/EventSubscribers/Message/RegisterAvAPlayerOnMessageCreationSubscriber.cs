using DAM.Bot.Common.Helper;
using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using DAM.Core.Requests.Commands.AvA;
using Discord;
using Discord.WebSocket;
using MediatR;

namespace DAM.Bot.EventSubscribers.Message
{
    public class RegisterAvAPlayerOnMessageCreationSubscriber : INotificationHandler<OnMessageCreatedEvent>
    {
        private readonly IAvAService _avaService;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IDiscordBotService _discordBotService;
        private readonly ISender _sender;
        public RegisterAvAPlayerOnMessageCreationSubscriber(IAvAService avaService, IAllianceManagementServiceAsync allianceManagementServiceAsync, IDiscordBotService discordBotService, ISender sender)
        {
            _avaService = avaService;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _discordBotService = discordBotService;
            _sender = sender;
        }
        public async Task Handle(OnMessageCreatedEvent notification, CancellationToken cancellationToken)
        {

            var channel = notification.SocketMessage.Channel as SocketGuildChannel;

            if (channel.GetChannelType().GetValueOrDefault() != ChannelType.PrivateThread &&
                channel.GetChannelType().GetValueOrDefault() != ChannelType.PublicThread)
            {
                return;
            }
            if (!notification.SocketMessage.Attachments.Any(a => a.ContentType.Contains("image")))
            {
                return;
            }
            var imageurl = notification.SocketMessage.Attachments.First(a => a.ContentType.Contains("image")).Url;

            var alliance = await _allianceManagementServiceAsync.GetAlliance(channel.Guild.Id.ToString());


            var ava = await _avaService.GetAvA(alliance.Id, channel.Id);
            if (ava == null)
            {
                return;
            }

            var request = new RegisterAvaPlayerCommand
            {
                AllianceId = alliance.Id,
                AvAId = ava.Id,
                DiscordMessageId = notification.SocketMessage.Id,
                PlayerDiscordId = notification.SocketMessage.Author.Id,
                ScreenAttachmentUrl = imageurl
            };
            await _sender.Send(request);

            await notification.SocketMessage.AddReactionAsync(EmojiHelper.CreateBookMarked);

        }


    }
}

