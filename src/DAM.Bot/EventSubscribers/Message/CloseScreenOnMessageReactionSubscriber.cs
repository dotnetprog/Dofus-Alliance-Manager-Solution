using DAM.Bot.Common.Helper;
using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Domain.Entities;
using DAM.Domain.Enums;
using DAM.Domain.JsonData;
using Discord;
using Discord.WebSocket;
using MediatR;
using Newtonsoft.Json;

namespace DAM.Bot.EventSubscribers.Message
{
    public class CloseScreenOnMessageReactionSubscriber : INotificationHandler<OnMessageReactEvent>
    {
        private readonly ISender _sender;

        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IScreenPostServiceAsync _screenPostService;
        private static readonly string[] AllowedEmojies = new string[]
        {
            EmojiHelper.GreenCheck.Name,
            EmojiHelper.RedCross.Name
        };

        public CloseScreenOnMessageReactionSubscriber(ISender sender, IAllianceManagementServiceAsync allianceManagementServiceAsync, IScreenPostServiceAsync screenPostService)
        {
            _sender = sender;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _screenPostService = screenPostService;
        }

        public async Task Handle(OnMessageReactEvent notification, CancellationToken cancellationToken)
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
            var userWhoReacted = (notification.Reaction.User.GetValueOrDefault() as IGuildUser);
            if (userWhoReacted == null)
            {
                return;
            }
            if (!userWhoReacted
                .RoleIds.Any(r =>
                r == alliance.AllianceConfiguration.ScreenApproverRoleId) &&
                !userWhoReacted.GuildPermissions.Administrator)
            {
                await notification.Message.RemoveReactionAsync(notification.Reaction.Emote, userWhoReacted);
                var messageid = await channel.SendMessageAsync($"🚨🚨 {userWhoReacted.Mention} Tu n'as pas les droits pour approuver/refuser les screens. Voir avec l'admin, ce message sera détruit dans 5 secondes");
                await Task.Delay(5000);
                await channel.DeleteMessageAsync(messageid);

                return;
            }
            var newstatus = MapStatus(notification.Reaction.Emote);
            if (!newstatus.HasValue)
            {
                return;
            }
            var command = new UpdateScreenPostValidationStatusCommand
            {
                ClosedByDiscordUserId = userWhoReacted.Id.ToString(),
                DiscordGuildId = channel.Guild.Id,
                Screenkey = existingScreen.Id.ToString(),
                Status = newstatus.Value
            };
            await _sender.Send(command);
        }
        private ScreenValidationResultStatus? MapStatus(IEmote emoji)
        {
            if (emoji.Name == EmojiHelper.GreenCheck.Name)
            {
                return ScreenValidationResultStatus.ManualyValid;
            }
            if (emoji.Name == EmojiHelper.RedCross.Name)
            {
                return ScreenValidationResultStatus.ManualyInvalid;
            }
            return null;

        }
        private bool CanBeProcessed(OnMessageReactEvent notification)
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

            return AllowedEmojies.Contains(notification.Reaction.Emote.Name);
        }
    }
}
