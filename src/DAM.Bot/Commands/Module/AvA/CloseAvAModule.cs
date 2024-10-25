using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;

namespace DAM.Bot.Commands.Module.AvA
{
    public class CloseAvAModule : BaseModule
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IAvAService _avaService;
        private readonly ILogger _logger;
        private static readonly ChannelType[] AllowedChannelTypes = new ChannelType[] {
            ChannelType.NewsThread,
            ChannelType.PublicThread,
            ChannelType.PrivateThread
        };
        public CloseAvAModule(IAvAService avaService,
            IAllianceManagementServiceAsync allianceManagementService,
           ILogger<CloseAvAModule> logger)

        {
            _logger = logger;
            _allianceManagementService = allianceManagementService;
            _avaService = avaService;
        }

        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        [SlashCommand("ava_close", "this command closes the KOTH/AvA")]
        public async Task Run(AvaResultState Resultat, int Points)
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
            ava.MontantPayeFixe = Points;


            if (ava == null)
            {
                await FollowupAsync(embed: BuildErrorMessage("Aucun AvA a été trouvé pour ce canal."), ephemeral: true);
                return;
            }
            var currentUser = this.Context.User as IGuildUser;
            var member = await _allianceManagementService.GetOrCreateAllianceMember(alliance.Id, currentUser);
            var participants = await this._avaService.GetMembers(ava.Id);
            ava.MontantPepitesTotal = participants.Count * Points;
            ava.MontantPepitesObtenu = 1;
            ava.ResultState = Resultat;
            ava.ClosedById = member.Id;

            await this._avaService.UpdateAva(ava);

            await ReplyAsync(embed: BuildSuccessMessage(currentUser,
                Resultat == AvaResultState.Win,
                ava.Titre, participants.Count, Points));


            var threadChannel = currentChannel as IThreadChannel;

            await threadChannel.ModifyAsync((props) => { props.Archived = true; props.Locked = true; });

        }
        private Embed BuildSuccessMessage(IGuildUser owner, bool isWin, string title, int NombreParticipants, int? montantFixe = null)
        {
            var IsFixe = montantFixe.HasValue;
            var endText = isWin ? "gagnée" : "perdue";
            var builder = new EmbedBuilder();
            builder.Title = $"AvA terminée";
            builder.WithAuthor(owner)
                   .WithColor(Color.DarkPurple)
                   .WithDescription($"L'AvA {title} a été {endText}.");
            builder.AddField("Nombre de participants", NombreParticipants.ToString(), true);



            builder.AddField("Montant par joueur", montantFixe.Value.ToString());

            builder.WithCurrentTimestamp();

            return builder.Build();
        }


    }
}
