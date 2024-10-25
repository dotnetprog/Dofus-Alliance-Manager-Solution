using DAM.Data.EntityFramework.Services;
using Discord.Interactions;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAM.Domain.Entities;
using DAM.Core.Abstractions.Services;

namespace DAM.Bot.Commands.Module.AvA
{
    public class RegisterAvAMemberModule : BaseModule
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IAvAService _avaService;
        private readonly ILogger _logger;
        private static readonly ChannelType[] AllowedChannelTypes = new ChannelType[] {
            ChannelType.NewsThread,
            ChannelType.PublicThread,
            ChannelType.PrivateThread
        };
        public RegisterAvAMemberModule(IAvAService avaService,
            IAllianceManagementServiceAsync allianceManagementService,
           ILogger<RegisterAvAMemberModule> logger)

        {
            _logger = logger;
            _allianceManagementService = allianceManagementService;
            _avaService = avaService;
        }
        [EnabledInDm(false)]
        [SlashCommand("ava_checkin", "Cette commande sert à s'enregistrer dans un canal AvA")]
        public async Task Run(IAttachment Screen)
        {
            await RespondAsync(embed: GetInProgressMessage(), ephemeral: true);


            var currentChannel = await this.Context.Client.GetChannelAsync(this.Context.Channel.Id);

            var channeltype = currentChannel.GetChannelType();
            if (!AllowedChannelTypes.Any(ct => ct == channeltype))
            {
                await FollowupAsync(embed: BuildErrorMessage("Vous ne pouvez pas faire cette commande sur ce type de canal."),ephemeral:true);
                return;
            }

            var alliance = await _allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());

            var ava = await this._avaService.GetAvA(alliance.Id, currentChannel.Id);

            if (ava == null)
            {
                await FollowupAsync(embed: BuildErrorMessage("Aucun AvA a été trouvé pour ce canal."),ephemeral:true);
                return;
            }

            if (ava.State == AvaState.Closed)
            {
                await FollowupAsync(embed: BuildErrorMessage("Impossible de s'enregistrer dans une AvA lorsqu'elle est terminée et fermée."));
                return;
            }
            var currentUser = this.Context.User as IGuildUser;
            var member = await _allianceManagementService.GetOrCreateAllianceMember(alliance.Id, currentUser);

            var participation = await CreateOrUpdateAvAMember(alliance, ava.Id, member, Screen.Url);

            var isNew = participation.DiscordMessageId == default(ulong);
            var message = await ReplyAsync(text:$"{currentUser.Mention}",embed: BuildSuccessMessage(currentUser, Screen.Url, isNew));

            if (!isNew)
            {
                this.Context.Channel.DeleteMessageAsync(participation.DiscordMessageId);
            }
            participation.DiscordMessageId = message.Id;
            await this._avaService.EditAvAMember(participation);

        }
        private Embed BuildSuccessMessage(IGuildUser owner,string imageurl,bool isNew)
        {
 
            var builder = new EmbedBuilder();
            var endtext = isNew ? "enregistrée" : "Modifiée";
            builder.Title = $"Participation {endtext}";
            builder.WithAuthor(owner)
                   .WithColor(Color.Purple).WithDescription($"{owner.Mention} est enregistré dans la liste des participants");
            
                builder.WithImageUrl(imageurl);
            
            builder.WithCurrentTimestamp();

            return builder.Build();
        }
        private async Task<AvaMember> CreateOrUpdateAvAMember(Alliance alliance,Guid AvaId,AllianceMember owner,string imageurl)
        {
            var participation = new AvaMember()
            {
                AvaId = AvaId,
                ImageUrl = imageurl,
                DiscordMessageId = default(ulong),
                MemberId = owner.Id
            };

            var existing = await _avaService.GetMember(AvaId, owner.Id);
            if(existing == null)
            {
                participation.Id = await this._avaService.RegisterAvAMember(participation);
            }
            else
            {
                participation.Id = existing.Id;
                participation.DiscordMessageId = existing.DiscordMessageId;
                await this._avaService.EditAvAMember(participation);
            }
            return participation;
        }
       

    }
}
