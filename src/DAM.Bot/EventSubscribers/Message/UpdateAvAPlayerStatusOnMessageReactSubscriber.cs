using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Discord.Message;
using DAM.Core.Requests.Commands.AvA;
using DAM.Domain.Entities;
using Discord;
using MediatR;

namespace DAM.Bot.EventSubscribers.Message
{
    public class UpdateAvAPlayerStatusOnMessageReactSubscriber : INotificationHandler<OnMessageReactEvent>
    {
        public const string GreenCheck = "✅";
        public const string RedCross = "❌";
        private readonly IAvAService _avaService;
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IDiscordBotService _discordBotService;
        private readonly ISender _sender;

        public UpdateAvAPlayerStatusOnMessageReactSubscriber(IAvAService avaService,
            IAllianceManagementServiceAsync allianceService,
            IDiscordBotService discordBotService,
            ISender sender)
        {
            _avaService = avaService;
            _allianceService = allianceService;
            _discordBotService = discordBotService;
            _sender = sender;
        }

        public async Task Handle(OnMessageReactEvent notification, CancellationToken cancellationToken)
        {
            var reaction = notification.Reaction;
            if (notification.Reaction.Channel is not IThreadChannel)
            {
                return;
            }
            var emote = reaction.Emote;

            //validate if reaction is either greencheck or red x.
            var ValidState = (AvAValidationState?)null;

            switch (emote.Name)
            {
                case GreenCheck:
                    ValidState = AvAValidationState.Approved;
                    break;
                case RedCross:
                    ValidState = AvAValidationState.Rejected;
                    break;
                default:
                    return;
            }
            var chnl = reaction.Channel as IThreadChannel;
            var botuser = await _discordBotService.GetCurrentGuildUser(chnl.GuildId);
            var originMessage = await chnl.GetMessageAsync(reaction.MessageId);



            //validate if user who reacted is allowed.
            if (chnl == null)
                return;
            var alliance = await _allianceService.GetAlliance(chnl.Guild.Id.ToString());
            if (alliance == null)
                return;
            var allianceConfig = await _allianceService.GetAllianceConfiguration(alliance.Id);
            if (allianceConfig == null)
            {
                return;
            }
            var userWhoReacted = (reaction.User.GetValueOrDefault() as IGuildUser);
            if (userWhoReacted == null)
            {
                return;
            }
            if (!userWhoReacted
                .RoleIds.Any(r =>
                r == allianceConfig.ScreenApproverRoleId))
            {
                return;
            }
            var ava = await _avaService.GetAvA(alliance.Id, reaction.Channel.Id);
            if (ava == null)
            {
                return;
            }
            var membre = await _allianceService.GetOrCreateAllianceMember(alliance.Id, userWhoReacted);
            var participation = await _avaService.GetMember(ava.Id, originMessage.Id);
            var request = new UpdateAvAMemberStatusCommand
            {
                AllianceId = alliance.Id,
                AvAMemberPresenceId = participation.Id,
                State = ValidState.Value,
                ValidatedById = membre.Id
            };
            await _sender.Send(request);

        }
    }
}
