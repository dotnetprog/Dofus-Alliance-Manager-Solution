using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;

namespace DAM.Bot.Commands.Module.AvA
{
    public class AvaModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IAvAService _avaService;
        private readonly ILogger _logger;
        const int SLOW_MODE_SECONDS = 30;
        public AvaModule(IAvAService avaService,
            IAllianceManagementServiceAsync allianceManagementService,
           ILogger<AvaModule> logger)

        {
            _logger = logger;
            _allianceManagementService = allianceManagementService;
            _avaService = avaService;
        }
        //[EnabledInDm(false)]
        //[DefaultMemberPermissions(GuildPermission.ManageGuild)]
        //[SlashCommand("ava_open_paievariable", "Cette commande sert à démarrer un AvA avec un paiement variable (%).")]
        //public async Task Run([Autocomplete(typeof(AutoCompleteZoneEnumModule))] string zone, [MinValue(1), MaxValue(100)] decimal Pourcentage, string message, string? SousZone = null, string? titre = null, IAttachment ScreenPrisme = null)
        //{
        //    await RespondAsync(embed: GetInProgressMessage(), ephemeral: true);
        //    var zoneValue = ZoneAvA.Inconnu;
        //    var zoneStr = zoneValue.ToString();
        //    if (int.TryParse(zone, out int zoneInteger))
        //    {
        //        zoneValue = (ZoneAvA)zoneInteger;
        //        zoneStr = GetEnumName(zoneValue);
        //    }

        //    var alliance = await _allianceManagementService.GetAlliance(Context.Guild.Id.ToString());
        //    if (alliance == null)
        //    {
        //        await FollowupAsync(embed: BuildErrorMessage("L'alliance n'est pas enregistré, veuillez utilisez /update_config."), ephemeral: true);
        //        return;
        //    }
        //    var hasConfig = (alliance.AllianceConfiguration?.Ava_DiscordForumChannelId.HasValue) ?? false;
        //    if (!hasConfig)
        //    {
        //        await FollowupAsync(embed: BuildErrorMessage("L'alliance ne possède pas de forum ava configuré, veuillez utilisez /update_config."), ephemeral: true);
        //        return;
        //    }
        //    var currentUser = await _allianceManagementService.GetOrCreateAllianceMember(alliance.Id, Context.User as IGuildUser);
        //    var forumChannel = alliance.AllianceConfiguration.Ava_DiscordForumChannelId.Value;
        //    var forum = (IForumChannel)Context.Client.GetChannel(forumChannel);

        //    var title = buildAvaTitle(zoneStr, SousZone, titre);


        //    var threadchannel = await forum.CreatePostAsync(title,
        //        ThreadArchiveDuration.OneWeek,
        //        SLOW_MODE_SECONDS,
        //        text: "@here",
        //        embed: BuildAvaMessage(title, zoneStr, Context.User, message, SousZone, ScreenPrisme?.Url));

        //    var ava = new Domain.Entities.AvA()
        //    {
        //        Titre = title,
        //        Zone = zoneValue,
        //        AllianceId = alliance.Id,
        //        CreatedById = currentUser.Id,
        //        DiscordForumChannelId = alliance.AllianceConfiguration.Ava_DiscordForumChannelId.Value,
        //        DiscordThreadChannelId = threadchannel.Id,
        //        ImageUrl = ScreenPrisme?.Url,
        //        PourcentagePaye = Pourcentage,
        //        ZoneAutres = SousZone
        //    };

        //    ava.Id = await _avaService.CreateAvA(ava);

        //    await FollowupAsync($"{threadchannel.Mention} a été créer. Ava Id = {ava.Id}", ephemeral: true);




        //}
        private string GetEnumName(ZoneAvA value)
        {
            var results = new List<AutocompleteResult>();

            var enumType = typeof(ZoneAvA);
            var field = enumType.GetFields().Where(f => f.FieldType.Name == enumType.Name).FirstOrDefault(f => f.Name == value.ToString());
            var valueAttributes =
                field.GetCustomAttributes(typeof(ChoiceDisplayAttribute), false);
            var description = valueAttributes.Length > 0 ? ((ChoiceDisplayAttribute)valueAttributes[0]).Name : field.Name;
            return description;



        }
        private string buildAvaTitle(string zone, string? souszone = null, string? titre = null)
        {
            var result = zone.ToString();
            if (!string.IsNullOrEmpty(souszone))
            {
                result += $" | {souszone}";
            }
            if (!string.IsNullOrEmpty(titre))
            {
                result += $" | {titre}";
            }
            return result;
        }
        private Embed BuildAvaMessage(string title, string zone, IUser owner, string message, string souszone, string? imageurl = null)
        {
            var zoneStr = $"{zone.ToString()}";
            if (!string.IsNullOrWhiteSpace(souszone))
            {
                zoneStr += $" ({souszone})";
            }
            var builder = new EmbedBuilder();
            builder.Title = title;
            builder.WithAuthor(owner)
                   .WithColor(Color.Purple).WithDescription(message);
            builder.AddField("Zone de combat", zoneStr);
            if (!string.IsNullOrEmpty(imageurl))
            {
                builder.WithImageUrl(imageurl);
            }
            builder.WithCurrentTimestamp();

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
        private Embed GetInProgressMessage()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle("Traitement en cours..");


            builder.WithColor(Color.Blue);
            return builder.Build();
        }


    }
}

