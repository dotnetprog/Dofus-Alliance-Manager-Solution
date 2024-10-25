using DAM.Bot.Common.Helper;
using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using DAM.Core.Requests.Commands;
using DAM.Domain.Entities;
using DAM.Domain.Enums;
using DAM.Domain.JsonData;
using Discord;
using Discord.WebSocket;
using MediatR;
using Newtonsoft.Json;

namespace DAM.Bot.EventSubscribers.Message
{
    public class CreateScreenOnMessageCreationSubscriber : INotificationHandler<OnMessageCreatedEvent>
    {
        private readonly ISender _sender;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;

        public CreateScreenOnMessageCreationSubscriber(ISender sender, IAllianceManagementServiceAsync allianceManagementServiceAsync)
        {
            _sender = sender;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
        }

        public async Task Handle(OnMessageCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (!CanBeProcessed(notification))
            {
                return;
            }

            var channel = notification.SocketMessage.Channel as SocketTextChannel;

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

            var userTagsObjects = notification.SocketMessage.Tags.Where(t => t.Type == TagType.UserMention).Select(t => (SocketGuildUser)t.Value).ToList();
            var command = new AddScreenPostCommand
            {
                EnemyCount = mapping.EnemyCount.Value,
                Target = ScreenPostTarget.Perco,
                DiscordChannelId = channel.Id,
                DiscordMessageId = notification.SocketMessage.Id,
                DiscordGuildId = channel.Guild.Id,
                ImageUrl = notification.SocketMessage.Attachments.First().Url,
                Type = mapping.Type.Value,
                OverridenCreatedByMemberId = notification.SocketMessage.Author.Id,
                AllianceMembers = userTagsObjects.Select(u => u.Id).ToArray()
            };
            if (notification.SocketMessage.Attachments.Count > 1)
            {
                command.ImagePrepUrl = notification.SocketMessage.Attachments.ElementAt(1).Url;
            }
            try
            {
                await _sender.Send(command);
            }
            catch (Exception ex)
            {
                await channel.SendMessageAsync("An Error has occured: " + ex.Message, false, null, null, null, new MessageReference(notification.SocketMessage.Id));
            }

            await notification.SocketMessage.AddReactionAsync(EmojiHelper.CreateBookMarked);
        }

        private bool CanBeProcessed(OnMessageCreatedEvent notification)
        {
            if (notification == null)
            {
                return false;
            }

            if (notification.SocketMessage.Channel is not SocketGuildChannel)
            {
                return false;
            }
            if (notification.SocketMessage.Author.IsBot)
            {
                return false;
            }
            var taggedUsers = notification.SocketMessage.Tags.Where(t => t.Type == TagType.UserMention).ToList();
            if (!notification.SocketMessage.Attachments.Any() ||
                !taggedUsers.Any(t => t.Type == TagType.UserMention) ||
                taggedUsers.Count > 5)
            {
                return false;
            }

            return true;



        }


    }

}
