using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using DAM.Domain.Enums;
using DAM.Domain.JsonData;
using Discord.WebSocket;
using MediatR;
using Newtonsoft.Json;

namespace DAM.Bot.EventSubscribers.Message
{
    public class DeleteScreenOnMessageDeletionSubscriber : INotificationHandler<OnMessageDeletedEvent>
    {
        private readonly ISender _sender;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IScreenPostServiceAsync _screenPostService;
        private readonly DiscordSocketClient _client;
        public DeleteScreenOnMessageDeletionSubscriber(ISender sender, IScreenPostServiceAsync screenPostService, IAllianceManagementServiceAsync allianceManagementServiceAsync, DiscordSocketClient client)
        {
            _sender = sender;
            _screenPostService = screenPostService;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _client = client;
        }


        public async Task Handle(OnMessageDeletedEvent notification, CancellationToken cancellationToken)
        {
            var channel = _client.GetChannel(notification.ChannelId) as SocketGuildChannel;
            var serverId = channel.Guild.Id;

            var alliance = await _allianceManagementServiceAsync.GetAlliance(serverId.ToString());
            if (alliance == null)
            {
                return;
            }
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
            var existingScreen = await _screenPostService.GetPost(notification.DiscordMessageId, false);
            if (existingScreen == null)
            {
                return;
            }
            await _screenPostService.DeletePost(existingScreen.Id);
        }
    }
}
