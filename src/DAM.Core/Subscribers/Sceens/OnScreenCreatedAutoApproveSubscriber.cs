using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Screens;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Domain.Entities;
using Discord;
using MediatR;

namespace DAM.Core.Subscribers.Sceens
{
    public class OnScreenCreatedAutoApproveSubscriber : INotificationHandler<ScreenCreatedEvent>
    {
        private readonly ISender _sender;

        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IScreenPostServiceAsync _screenPostService;

        private readonly IDiscordBotService _discordBotService;
        public static Emoji GreenCheck = new Emoji("✅");
        public OnScreenCreatedAutoApproveSubscriber(ISender sender,
            IAllianceManagementServiceAsync allianceManagementServiceAsync,
            IScreenPostServiceAsync screenPostService,
            IDiscordBotService discordBotService)
        {
            _sender = sender;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _screenPostService = screenPostService;
            _discordBotService = discordBotService;
        }
        public async Task Handle(ScreenCreatedEvent notification, CancellationToken cancellationToken)
        {
            var screen = notification.ScreenPost;
            if (screen.EnemyCount != 0 || screen.Type != ScreenPostType.Attack)
            {
                return;
            }

            var createdby = await _allianceManagementServiceAsync.GetMember(screen.CreatedByMemberId.Value);
            var allianceOwner = await _allianceManagementServiceAsync.GetAlliance(createdby.AllianceId.Value);
            var isAutoValidationEnabled = allianceOwner.AllianceConfiguration?.AutoValidateNoDef ?? false;
            if (!isAutoValidationEnabled)
            {
                return;
            }
            var allianceDiscordId = ulong.Parse(allianceOwner.DiscordGuildId);
            var mainUser = await _discordBotService.GetCurrentGuildUser(allianceDiscordId);

            var command = new UpdateScreenPostValidationStatusCommand
            {
                ClosedByDiscordUserId = mainUser.Id.ToString(),
                DiscordGuildId = allianceDiscordId,
                Screenkey = screen.Id.ToString(),
                Status = ScreenValidationResultStatus.ManualyValid
            };
            await _sender.Send(command);

            var txtchannel = (ITextChannel)(await _discordBotService.GetChannel(allianceDiscordId, screen.DiscordChannelId.Value));

            await _discordBotService.React(txtchannel, screen.DiscordMessageId.Value, GreenCheck);


        }
    }
}
