using Alachisoft.NCache.JNIBridge.Net;
using AnkamaWebClient.Abstractions;
using DAM.Bot.Common.Extensions;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.JsonData;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module.Ankama
{
    public class WhoIsModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly ILogger _logger;
        private readonly IMembreService _memberService;
        private readonly IAnkamaService _ankamaService;
        public WhoIsModule(IMembreService membreService, IAnkamaService ankamaService, IAllianceManagementServiceAsync allianceManagementService,
           ILogger<WhoIsModule> logger)

        {
            _memberService = membreService;
            _logger = logger;
            _allianceManagementService = allianceManagementService;
            _ankamaService = ankamaService;
        }
        private Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
      
      


        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        [SlashCommand("info-joueur", "Cette commande sert à avoir des informations relatifs à un membre via son tag discord")]
        public async Task RunUser(IGuildUser User)
        {

            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("L'Alliance n'a pas été enregistré. Faites /update_config d'abord."), ephemeral: true);
                return;
            }
            var member = await _allianceManagementService.GetMember(alliance.Id,User.Id.ToString());
            if(member == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"{User.Username} n'a pas été trouvé dans la base de donnée."), ephemeral: true);
                return;
            }

            var pseudos = await _memberService.GetPseudos(member.Id);

            if (!pseudos.Any())
            {
                await this.FollowupAsync(embed: BuildNoDataFound(), ephemeral: true);
                return;
            }


            var embeds = new List<Embed>();
            foreach (var p in pseudos.Take(10))
            {
                var builder = new EmbedBuilder();
                builder.WithAuthor(p.CreatedBy.Nickname ?? p.CreatedBy.Alias)
                       .WithTitle($"Resultats pour {User.Username}")
                       .WithDescription($"Pseudo Ankama: {p.Pseudo}").WithUrl(p.AnkadexUrl);

                var data = p.GetData().Batch(10).ToArray();
                for(var i = 0; i< data.Length; i++)
                {
                    var batch = data[i];

                    builder.AddField($"Liste des personnages #{i.ToString()}",
                        string.Join(",", batch.Select(b => $"{b.NomPersonnage}[{b.Serveur}]")));

                }
                builder.WithCurrentTimestamp().WithColor(Color.Default);
                embeds.Add(builder.Build());
            }
            await this.FollowupAsync(embeds: embeds.ToArray());

        }
        private Embed BuildNoDataFound()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Aucune donnée trouvé");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        [SlashCommand("info-pseudo", "Cette commande sert à avoir des informations relatifs à un membre via son pseudo ankama")]
        public async Task RunAnkamaPseudo(string? AnkamaPseudo)
        {
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("L'Alliance n'a pas été enregistré. Faites /update_config d'abord."), ephemeral: true);
                return;
            }
            var IsValid = await _ankamaService.ValidatePseudo(AnkamaPseudo);

            if (!IsValid)
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"Le pseudo {AnkamaPseudo} ne correspond pas à un pseudo valide."));
                return;
            }

            var members = await _memberService.RechercheParPseudo(alliance.Id, AnkamaPseudo.ToLowerInvariant());

            if (!members.Any())
            {
                await this.FollowupAsync(embed: BuildNoDataFound(), ephemeral: true);
                return;
            }

            var builder = new EmbedBuilder();
            builder.WithAuthor(this.Context.User)
                .WithTitle($"Résultat pour {AnkamaPseudo}")
                .WithDescription(string.Join(",", members.Select(m => m.Nickname ?? m.Alias)));
            builder.WithColor(Color.Default).WithCurrentTimestamp();

            await this.FollowupAsync(embed: builder.Build());


        }
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        [SlashCommand("info-personnage", "Cette commande sert à trouver un membre discord avec un nom de personnage.")]
        public async Task RunPersonnage(ServeurDofus Serveur,[MinLength(3)]string? NomPersonnage)
        {
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("L'Alliance n'a pas été enregistré. Faites /update_config d'abord."), ephemeral: true);
                return;
            }


            var results = await _memberService.RechercheParPersonnage(alliance.Id, NomPersonnage.ToLowerInvariant(), Serveur.ToString().ToLowerInvariant());
            if (!results.Any())
            {
                await this.FollowupAsync(embed: BuildNoDataFound(), ephemeral: true);
                return;
            }
            var builder = new EmbedBuilder();
            builder.WithAuthor(this.Context.User)
                .WithTitle($"Résultat pour {NomPersonnage}[{Serveur.ToString()}]")
                .WithDescription(string.Join(",", results.Select(m => m.Nickname ?? m.Alias)));
            builder.WithColor(Color.Default).WithCurrentTimestamp();

            await this.FollowupAsync(embed: builder.Build());

        }
        private Embed BuildErrorMessage(string message)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Une erreur est survenue.")
                .WithDescription(message);

            builder.WithColor(Color.Red);
            return builder.Build();
        }
    }
}
