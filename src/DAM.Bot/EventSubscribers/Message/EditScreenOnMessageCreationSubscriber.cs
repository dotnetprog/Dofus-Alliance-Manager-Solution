using DAM.Bot.Common.Helper;
using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Domain.Enums;
using DAM.Domain.JsonData;
using Discord;
using Discord.WebSocket;
using MediatR;
using Newtonsoft.Json;

namespace DAM.Bot.EventSubscribers.Message
{
    public class EditScreenOnMessageCreationSubscriber : INotificationHandler<OnMessageUpdatedEvent>
    {
        private readonly ISender _sender;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IScreenPostServiceAsync _screenPostService;
        public EditScreenOnMessageCreationSubscriber(ISender sender, IAllianceManagementServiceAsync allianceManagementServiceAsync, IScreenPostServiceAsync screenPostServiceAsync)
        {
            _sender = sender;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _screenPostService = screenPostServiceAsync;
        }


        public async Task Handle(OnMessageUpdatedEvent notification, CancellationToken cancellationToken)
        {
            if (!CanBeProcessed(notification))
            {
                return;
            }

            var channel = notification.Message.Channel as SocketTextChannel;

            var alliance = await _allianceManagementServiceAsync.GetAlliance(channel.Guild.Id.ToString());

            if (alliance == null || alliance.AllianceConfiguration.BotScreenBehaviorType != BotScreenBehaviorType.ChannelEnnemyCountMapping)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(alliance.AllianceConfiguration.BehaviorScreenConfigJSONData))
            {
                return;
            }
            var config = JsonConvert.DeserializeObject<AutoMappingChannelEnemyCountConfig>(alliance.AllianceConfiguration.BehaviorScreenConfigJSONData);

            var mapping = config.GetCountFromChannel(channel.Id.ToString());
            if (mapping == null || !mapping.EnemyCount.HasValue)
            {
                return;
            }

            var existingScreen = await _screenPostService.GetPost(notification.Message.Id, false);
            if (existingScreen == null)
            {
                return;
            }


            var userTagsObjects = notification.Message.Tags.Where(t => t.Type == TagType.UserMention).Select(t => (IUser)t.Value).ToList();
            var command = new UpdateScreenPostCommand
            {
                AllianceMembers = userTagsObjects.Select(u => u.Id).ToArray(),
                DiscordGuildId = channel.Guild.Id,
                DiscordMessageId = notification.Message.Id,
                ImageUrl = notification.Message.Attachments.First().Url,

            };
            if (notification.Message.Attachments.Count > 1)
            {
                command.ImagePrepUrl = notification.Message.Attachments.ElementAt(1).Url;
            }

            try
            {
                await _sender.Send(command);
            }
            catch (Exception ex)
            {
                await channel.SendMessageAsync("An Error has occured: " + ex.Message, false, null, null, null, new MessageReference(notification.Message.Id));
            }
            await notification.Message.AddReactionAsync(EmojiHelper.EditBookMarked);
        }
        private bool CanBeProcessed(OnMessageUpdatedEvent notification)
        {
            if (notification == null)
            {
                return false;
            }

            if (notification.Message.Channel is not ITextChannel)
            {
                return false;
            }
            if (notification.Message.Author.IsBot)
            {
                return false;
            }
            var taggedUsers = notification.Message.Tags.Where(t => t.Type == TagType.UserMention).ToList();
            if (!notification.Message.Attachments.Any() ||
                !taggedUsers.Any(t => t.Type == TagType.UserMention) ||
                taggedUsers.Count > 5)
            {
                return false;
            }



            return true;



        }
    }
}
