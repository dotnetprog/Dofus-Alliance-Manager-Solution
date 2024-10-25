using DAM.Bot.Commands.AutoComplete;
using DAM.Core.Abstractions.Services;
using DAM.Data.EntityFramework.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module.AvA
{
    public class DeleteAvAModule : BaseModule
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IAvAService _avaService;
        private readonly ILogger _logger;
        private static readonly ChannelType[] AllowedChannelTypes = new ChannelType[] {
            ChannelType.NewsThread,
            ChannelType.PublicThread,
            ChannelType.PrivateThread
        };
        public DeleteAvAModule(IAvAService avaService,
            IAllianceManagementServiceAsync allianceManagementService,
           ILogger<DeleteAvAModule> logger)

        {
            _logger = logger;
            _allianceManagementService = allianceManagementService;
            _avaService = avaService;
        }
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        [SlashCommand("ava_delete", "Cette commande sert à supprimer une AvA")]
        public async Task Run()
        {
            await RespondAsync(embed: GetInProgressMessage(), ephemeral: true);


            var currentChannel = await this.Context.Client.GetChannelAsync(this.Context.Channel.Id);

            var channeltype = currentChannel.GetChannelType();
            if (!AllowedChannelTypes.Any(ct => ct == channeltype))
            {
                await FollowupAsync(embed: BuildErrorMessage("Vous ne pouvez pas faire cette commande sur ce type de canal."), ephemeral: true);
                return;
            }

            var alliance = await _allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());

            var ava = await this._avaService.GetAvA(alliance.Id, currentChannel.Id);

            if(ava  == null)
            {
                await FollowupAsync(embed: BuildErrorMessage("Aucun AvA a été trouvé pour ce canal."), ephemeral: true);
                return;
            }

            if (ava.ResultState.HasValue)
            {
                await FollowupAsync(embed: BuildErrorMessage("Impossible de supprimer une AvA lorsqu'elle est terminée."), ephemeral: true);
                return;
            }
           
            await this._avaService.DeleteAvA(alliance.Id, ava.Id);
            await ((IThreadChannel)currentChannel).DeleteAsync();
           

        }
       
    }
}
