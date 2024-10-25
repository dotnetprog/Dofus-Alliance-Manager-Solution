using DAM.Bot.Commands.ComplexTypes;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module.Screen
{
    public class DefScreenModule : T5ScreenBaseModule<ScreenCommandInput>
    {
        public DefScreenModule(
            IScreenPostServiceAsync ScreenService,
            IAllianceManagementServiceAsync allianceManagementService,
            ILogger<DefScreenModule> logger) : base(ScreenPostType.Defense, ScreenService, allianceManagementService, logger)
        {
        }
        [EnabledInDm(false)]
        [SlashCommand("defense_screen", "Commande pour enregistrer une défense perco/prisme", false, RunMode.Async)]
        public override async Task Run([ComplexParameter] ScreenCommandInput data)
        {
            await base.Run(data);
        }
        protected override Embed[] BuildEmbed(ScreenCommandInput data, Guid screenId)
        {
            var attaquants = data.GetAttaquants().ToList();
            var embed = new EmbedBuilder()
            {
                Title = $"Défense {data.AttaqueType} avec {attaquants.Count} membre(s) enregistré(s)",
                Description = $"{Context.User.Mention} Le screen est enregistré, la validation se fera prochainement."

            };
            var screentag = $"{attaquants.Count}vs{data.NombreEnemies}";
            embed.WithAuthor(Context.User).AddField("Identifiant du screen", screenId.ToString())
                 .AddField("Participants:", string.Join(",", attaquants.Select(a => a.Mention).Distinct().ToArray()))
                 .AddField("Mode", screentag);

            if (data.Enemy != null)
            {
                embed.AddField("Alliance adverse:", data.Enemy.Name);
            }
            if (!string.IsNullOrWhiteSpace(data.Description))
            {
                embed.AddField("Note", data.Description);
            };
            embed.WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithImageUrl(data.file.Url);
            return new Embed[] { embed.Build() };
        }
    }
}
