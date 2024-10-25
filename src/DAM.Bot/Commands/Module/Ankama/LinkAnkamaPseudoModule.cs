using AnkamaWebClient.Abstractions;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.JsonData;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace DAM.Bot.Commands.Module.Ankama
{
    public class LinkAnkamaPseudoModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly ILogger _logger;
        private readonly IMembreService _memberService;
        private readonly IAnkamaService _ankamaService;
        public LinkAnkamaPseudoModule(IMembreService membreService, IAnkamaService ankamaService, IAllianceManagementServiceAsync allianceManagementService,
           ILogger<LinkAnkamaPseudoModule> logger)

        {
            _memberService = membreService;
            _logger = logger;
            _allianceManagementService = allianceManagementService;
            _ankamaService = ankamaService;
        }
        const string ANKAMA_URL = "https://account.ankama.com/fr/profil-ankama/";
        [EnabledInDm(false)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        [SlashCommand("link-ankama-pseudo", "Cette commande sert à un pseudo ankama à un membre de l'alliance.")]
        public async Task Run(IGuildUser User, string AnkamaPseudo)
        {
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var IsValid = await _ankamaService.ValidatePseudo(AnkamaPseudo);

            if (!IsValid)
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"Le pseudo {AnkamaPseudo} ne correspond pas à un pseudo valide."), ephemeral: true);
                return;
            }

            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("L'Alliance n'a pas été enregistré. Faites /update_config d'abord."), ephemeral: true);
                return;
            }
            var currentUser = await this._allianceManagementService.GetOrCreateAllianceMember(alliance.Id, this.Context.User as IGuildUser);


            var membre = await this._allianceManagementService.GetOrCreateAllianceMember(alliance.Id, User);
            var data = await this._memberService.RechercheParPseudo(alliance.Id, AnkamaPseudo);
            if (data.Any(m => m.Id != membre.Id))
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"Le pseudo {AnkamaPseudo} est déjà associé à un membre"), ephemeral: true);
                return;
            }

            var pseudos = await _ankamaService.SearchAnkadex(AnkamaPseudo.ToLowerInvariant());
            if (pseudos == null)
            {
                await this.FollowupAsync(embed: BuildNoDataFound(), ephemeral: true);
                return;
            }
            var parsedPseudo = await _ankamaService.ParsePseudo(AnkamaPseudo.ToLowerInvariant());

            var json = JsonConvert.SerializeObject(pseudos.Where(p => p != null).ToArray());

            var dataPseudo = new AnkamaPseudo
            {
                AllianceMemberId = membre.Id,
                AnkadexUrl = ANKAMA_URL + parsedPseudo,
                CreatedById = currentUser.Id,
                LastRefreshedOn = DateTime.UtcNow,
                Pseudo = AnkamaPseudo.ToLowerInvariant(),
                PseudoData = json
            };

            await this._memberService.AddPseudo(dataPseudo);


            await this.ReplyAsync(embed: BuildSuccessMessage(User, dataPseudo, pseudos));

        }
        [EnabledInDm(false)]
        [SlashCommand("link-myself", "Cette commande sert à lier ton pseudo ankama à ton compte discord")]
        public async Task RunMySelf(string AnkamaPseudo)
        {
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var IsValid = await _ankamaService.ValidatePseudo(AnkamaPseudo);

            if (!IsValid)
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"Le pseudo {AnkamaPseudo} ne correspond pas à un pseudo valide."), ephemeral: true);
                return;
            }

            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("L'Alliance n'a pas été enregistré. Faites /update_config d'abord."), ephemeral: true);
                return;
            }
            var membre = await this._allianceManagementService.GetOrCreateAllianceMember(alliance.Id, this.Context.User as IGuildUser);



            var data = await this._memberService.RechercheParPseudo(alliance.Id, AnkamaPseudo);
            if (data.Any(m => m.Id != membre.Id))
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"Le pseudo {AnkamaPseudo} est déjà associé à un membre"), ephemeral: true);
                return;
            }

            var pseudos = await _ankamaService.SearchAnkadex(AnkamaPseudo.ToLowerInvariant());
            if (pseudos == null)
            {
                await this.FollowupAsync(embed: BuildNoDataFound(), ephemeral: true);
                return;
            }
            var parsedPseudo = await _ankamaService.ParsePseudo(AnkamaPseudo.ToLowerInvariant());

            var json = JsonConvert.SerializeObject(pseudos.Where(p => p != null).ToArray());

            var dataPseudo = new AnkamaPseudo
            {
                AllianceMemberId = membre.Id,
                AnkadexUrl = ANKAMA_URL + parsedPseudo,
                CreatedById = membre.Id,
                LastRefreshedOn = DateTime.UtcNow,
                Pseudo = AnkamaPseudo.ToLowerInvariant(),
                PseudoData = json
            };

            await this._memberService.AddPseudo(dataPseudo);


            await this.ReplyAsync(embed: BuildSuccessMessage(this.Context.User as IGuildUser, dataPseudo, pseudos));
        }
        [EnabledInDm(false)]
        [SlashCommand("unlink-myself", "Cette commande sert à délier un pseudo ankama à ton compte discord")]
        public async Task ClearMyself(string AnkamaPseudo)
        {
            await this.RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
            var IsValid = await _ankamaService.ValidatePseudo(AnkamaPseudo);

            if (!IsValid)
            {
                await this.FollowupAsync(embed: BuildErrorMessage($"Le pseudo {AnkamaPseudo} ne correspond pas à un pseudo valide."), ephemeral: true);
                return;
            }

            var alliance = await this._allianceManagementService.GetAlliance(this.Context.Guild.Id.ToString());
            if (alliance == null)
            {
                await this.FollowupAsync(embed: BuildErrorMessage("L'Alliance n'a pas été enregistré. Faites /update_config d'abord."), ephemeral: true);
                return;
            }
            var membre = await this._allianceManagementService.GetOrCreateAllianceMember(alliance.Id, this.Context.User as IGuildUser);

            var pseudoMember = await this._memberService.GetPseudos(membre.Id);
            if (!pseudoMember.Any(p => p.Pseudo == AnkamaPseudo.ToLowerInvariant()))
            {

                await this.FollowupAsync(embed: BuildNoDataFound(), ephemeral: true);
                return;
            }

            await _memberService.DeletePseudo(membre.Id, AnkamaPseudo);

            await this.ReplyAsync(embed: BuildSuccessMessageRemove(this.Context.User as IGuildUser, pseudoMember.FirstOrDefault(p => p.Pseudo == AnkamaPseudo.ToLowerInvariant())));
        }

        private Embed BuildNoDataFound()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Aucune donnée trouvé");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
        private Embed BuildErrorMessage(string message)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Une erreur est survenue.")
                .WithDescription(message);

            builder.WithColor(Color.Red);
            return builder.Build();
        }
        private Embed BuildSuccessMessageRemove(IGuildUser user, AnkamaPseudo parent)
        {
            var builder = new EmbedBuilder();
            var pseudos = parent.GetData();
            builder.WithTitle($"{pseudos.Count()} personnage(s) ont été délié à {user.Nickname ?? user.DisplayName ?? user.Username}");
            builder.WithDescription($"Le pseudo {parent.Pseudo} et les personnes ont été directement retiré.{user.Mention}");

            var pseudoMax = pseudos.Take(5);

            foreach (var p in pseudoMax)
            {
                builder.AddField($"{p.NomPersonnage}[{p.Serveur}]", "Guilde: " + p.Guild);
            }

            builder.WithColor(Color.Green).WithCurrentTimestamp();
            return builder.Build();
        }
        private Embed BuildSuccessMessage(IGuildUser user, AnkamaPseudo parent, IEnumerable<AnkamaPseudoData> pseudos)
        {
            var builder = new EmbedBuilder();

            builder.WithTitle($"{pseudos.Count()} personnage(s) ont été lié à {user.Nickname ?? user.DisplayName ?? user.Username}").WithUrl(parent.AnkadexUrl);
            builder.WithDescription($"Le pseudo {parent.Pseudo} et les personnes ont été directement associé.{user.Mention}");

            var pseudoMax = pseudos.Take(5);

            foreach (var p in pseudoMax)
            {
                builder.AddField($"{p.NomPersonnage}[{p.Serveur}]", "Guilde: " + p.Guild);
            }

            builder.WithColor(Color.Green).WithCurrentTimestamp();
            return builder.Build();
        }

        private Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }
    }
}
