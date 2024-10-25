using DAM.Core.Abstractions.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module.Screen
{
    public class DeleteScreenPostModule : InteractionModuleBase<SocketInteractionContext>
    {
        IAllianceManagementServiceAsync _allianceManagementService;
        IScreenPostServiceAsync _screenPostService;
        public DeleteScreenPostModule(IScreenPostServiceAsync screenService, IAllianceManagementServiceAsync allianceManagementService)
        {
            _screenPostService = screenService;
            _allianceManagementService = allianceManagementService;
        }
        private Embed BuildErrorMessage(string message)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Une erreur est survenue.")
                .WithDescription(message);

            builder.WithColor(Color.Red);
            return builder.Build();
        }
        private Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
        private Embed BuildSuccessMessage(string message)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Screen retiré de la base de donnée.").WithDescription(message);

            builder.WithColor(Color.Green).WithCurrentTimestamp();
            return builder.Build();
        }
        [EnabledInDm(false)]
        [SlashCommand("delete_screen", "Cette commande sert à supprimer un screen. Seule le proprio peut supprimer le screen.")]
        public async Task Run(string screen_id)
        {
            await RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            if (Guid.TryParse(screen_id, out Guid postId))
            {
                var post = await _screenPostService.GetPost(postId, false);
                if (post == null)
                {
                    await FollowupAsync(embed: BuildErrorMessage($"aucun screen trouvé pour {screen_id}"), ephemeral: true);
                    return;
                }
                var member = await _allianceManagementService.GetMember(post.CreatedByMemberId.Value);
                if (member.DiscordId != Context.User.Id.ToString())
                {
                    await FollowupAsync(embed: BuildErrorMessage($"Seule le propriétaire peut supprimer le screen."), ephemeral: true);
                    return;
                }
                var deleteEmbedInDiscord = Task.Run(async () =>
                {

                    if (post.DiscordChannelId.HasValue && post.DiscordMessageId.HasValue)
                    {
                        var chnl = await Context.Client.GetChannelAsync(post.DiscordChannelId.Value);
                        if (chnl != null && chnl is SocketTextChannel txtchnl)
                        {
                            var message = await txtchnl.GetMessageAsync(post.DiscordMessageId.Value);
                            if (message != null)
                            {
                                await message.DeleteAsync();
                            }
                        }
                    }


                });


                await Task.WhenAll(_screenPostService.DeletePost(postId), deleteEmbedInDiscord);
                await ReplyAsync(embed: BuildSuccessMessage($"Le screenid {postId} n'existe plus."));
            }
            else
            {
                await FollowupAsync(embed: BuildErrorMessage($"{screen_id} doit être sous un format guid."), ephemeral: true);
            }
        }

    }
}
