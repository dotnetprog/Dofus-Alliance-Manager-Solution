using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module
{
    public class UpdateConfigModule : InteractionModuleBase<SocketInteractionContext>
    {
        IAllianceManagementServiceAsync _allianceManagementService;
        public UpdateConfigModule(IAllianceManagementServiceAsync allianceManagementService) {
            this._allianceManagementService = allianceManagementService;
        }
        private Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [SlashCommand("update_config","Cette commande sert à configurer la configuration de l'alliance auprès du bot.",false,RunMode.Async)]
        public async Task Run([ChannelTypes(ChannelType.Text)]IChannel CanalAttaqueScreen, 
            [ChannelTypes(ChannelType.Text)] IChannel CanalDefenceScreen,
            [ChannelTypes(ChannelType.Text)] IChannel CanalRapportPepite,
            IRole RoleApprobateur,
            bool ScreenPrepaRequis,bool AllianceEnemisRequis, [ChannelTypes(ChannelType.Forum)] IChannel CanalAvAForum = null)
        {
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var config = new AllianceConfiguration();
            config.AtkScreen_DiscordChannelId = CanalAttaqueScreen.Id;
            config.DefScreen_DiscordChannelId = CanalDefenceScreen.Id;
            config.ScreenApproverRoleId = RoleApprobateur.Id;
            config.Rapport_DiscordChannelId= CanalRapportPepite.Id;
            config.IsAllianceEnemyRequired = AllianceEnemisRequis;
            config.IsScreenPrepaRequired = ScreenPrepaRequis;
            if (CanalAvAForum != null)
            {
                config.Ava_DiscordForumChannelId = CanalAvAForum.Id;
            }
            

            var alliance = await ConfigureAlliance();
            config.Id = alliance.AllianceConfigurationId ?? Guid.Empty;
            await this._allianceManagementService.UpdateAllianceConfiguration(alliance.Id, config);
            var embed = new EmbedBuilder()
            {
                Title = $"Configuration de l'alliance"

            };
            var screenprepValue = ScreenPrepaRequis ? "Oui" : "Non";
            var allianceEnemyValue = AllianceEnemisRequis ? "Oui" : "Non";
            embed.AddField("Canal défense", $"{CanalDefenceScreen.Name}", true).
                AddField("Canal attaque", $"{CanalAttaqueScreen.Name}", true)
                .AddField("Canal Rapport Pépite", $"{CanalRapportPepite.Name}", false)
                .AddField("Screen préparation obligatoire ?", $"{screenprepValue}", false)
                .AddField("Alliance Enemie requis ?", $"{allianceEnemyValue}", false)
                .WithAuthor(Context.User)
                .WithColor(Color.Magenta)
                .WithCurrentTimestamp();
            await this.ReplyAsync(embed:embed.Build());
        }
        public async Task<Alliance> ConfigureAlliance()
        {
            var alliance = await _allianceManagementService.GetAlliance(Context.Guild.Id.ToString());
           
            if (alliance == null)
            {
                alliance = await _allianceManagementService.RegisterAlliance(Context.Guild.Id.ToString(), Context.Guild.Name);
            }
         
  
            return alliance;
        }



    }
}
