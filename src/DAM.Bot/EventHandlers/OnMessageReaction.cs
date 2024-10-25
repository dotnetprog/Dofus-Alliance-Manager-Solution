using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.WebSocket;

namespace DAM.Bot.EventHandlers
{
    public class OnMessageReaction
    {
        public const string GreenCheck = "✅";
        public const string RedCross = "❌";
        public static async Task Handle(DiscordSocketClient client, SocketReaction reaction,
            IScreenPostServiceAsync screenService,
            ILogger logger,
            IAllianceManagementServiceAsync allianceService)
        {
            if (reaction.Channel is not SocketTextChannel || reaction.Channel is IThreadChannel)
            {
                return;
            }
            var emote = reaction.Emote;
            logger.LogDebug($"emote: {emote.Name}");
            //validate if reaction is either greencheck or red x.
            var ValidState = (ScreenValidationResultStatus?)null;

            switch (emote.Name)
            {
                case GreenCheck:
                    ValidState = ScreenValidationResultStatus.ManualyValid;
                    break;
                case RedCross:
                    ValidState = ScreenValidationResultStatus.ManualyInvalid;
                    break;
                default:
                    return;
            }
            var chnl = reaction.Channel as SocketTextChannel;
            var botuser = client.CurrentUser;
            var originMessage = await chnl.GetMessageAsync(reaction.MessageId);

            logger.LogDebug($"botuser.Id: {botuser}");
            logger.LogDebug($"Author.Id: {originMessage.Author.Id}");
            if (originMessage.Author.Id != botuser.Id)
            {
                return;
            }
            //validate if user who reacted is allowed.
            if (chnl == null)
                return;
            var alliance = await allianceService.GetAlliance(chnl.Guild.Id.ToString());
            if (alliance == null)
                return;
            var allianceConfig = await allianceService.GetAllianceConfiguration(alliance.Id);
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

            var closedby = await allianceService.GetOrCreateAllianceMember(alliance.Id, userWhoReacted);





            //validate if the messageid is related to a screenpost.
            var post = await screenService.GetPost(reaction.MessageId, false);

            if (post == null) return;
            await screenService.SetScreenValidationResult(post.Id, ValidState.Value, closedby.Id);



        }

        public static async Task Handle(DiscordSocketClient client, SocketReaction reaction,
          IAvAService avaService,
          ILogger logger,
          IAllianceManagementServiceAsync allianceService)
        {
            if (reaction.Channel is not IThreadChannel)
            {
                return;
            }
            var emote = reaction.Emote;
            logger.LogDebug($"emote: {emote.Name}");
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
            var botuser = client.CurrentUser;
            var originMessage = await chnl.GetMessageAsync(reaction.MessageId);

            logger.LogDebug($"botuser.Id: {botuser}");
            logger.LogDebug($"Author.Id: {originMessage.Author.Id}");
            if (originMessage.Author.Id != botuser.Id)
            {
                return;
            }
            //validate if user who reacted is allowed.
            if (chnl == null)
                return;
            var alliance = await allianceService.GetAlliance(chnl.Guild.Id.ToString());
            if (alliance == null)
                return;
            var allianceConfig = await allianceService.GetAllianceConfiguration(alliance.Id);
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

            var membre = await allianceService.GetOrCreateAllianceMember(alliance.Id, userWhoReacted);


            var ava = await avaService.GetAvA(alliance.Id, reaction.Channel.Id);

            //validate if the messageid is related to a screenpost.
            var participation = await avaService.GetMember(ava.Id, originMessage.Id);
            participation.ValidationState = ValidState;
            participation.ValidatedById = membre.Id;

            await avaService.EditAvAMember(participation);



        }

    }
}
