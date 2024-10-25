using DAM.Bot.Commands.ComplexTypes;
using DAM.Bot.Common.Helper;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.Module.Screen
{
    public class AtkScreenCommands : T5ScreenBaseModule<AtkScreenCommandInput>
    {

        public AtkScreenCommands(IScreenPostServiceAsync ScreenService,
            IAllianceManagementServiceAsync allianceManagementService,
            ILogger<AtkScreenCommands> logger)
            : base(ScreenPostType.Attack, ScreenService, allianceManagementService, logger)
        {

        }
        [EnabledInDm(false)]
        [SlashCommand("attaque_screen", "Commande pour enregistrer une attaque perco/prisme", false, RunMode.Async)]
        public override async Task Run([ComplexParameter] AtkScreenCommandInput data)
        {

            await base.Run(data);
        }
        public override async Task<bool> OnPreValidationExecute(Alliance alliance, AtkScreenCommandInput data)
        {
            if (alliance.AllianceConfiguration != null)
            {
                var isScreenPrepaRequis = alliance.AllianceConfiguration.IsScreenPrepaRequired ?? false;
                if (isScreenPrepaRequis && data.ScreenPrepa == null && data.NombreEnemies < 1)
                {
                    await FollowupAsync(embed: BuildErrorMessage("Le screen préparation est obligatoire lorsqu'il n'y a aucune défense."));
                    return await Task.FromResult(false);
                }
            }
            return await base.OnPreValidationExecute(alliance, data);
        }
        protected override Embed[] BuildEmbed(AtkScreenCommandInput data, Guid screenId)
        {
            var attaquants = data.GetAttaquants().ToList();
            var embed = new EmbedBuilder()
            {
                Title = $"Attaque {data.AttaqueType} avec {attaquants.Count} membre(s) enregistré(s)",
                Description = $"{Context.User.Mention} Le screen est enregistré, la validation se fera prochainement."

            };


            var screentag = $"{attaquants.Count}vs{data.NombreEnemies}";
            embed.WithAuthor(Context.User)
                .AddField("Identifiant du screen", screenId.ToString())
                .AddField("Participants:", string.Join(",", attaquants.Select(a => a.Mention).Distinct().ToArray()))
                .AddField("Mode", screentag).WithColor(Color.Orange);
            if (data.Enemy != null)
            {
                embed.AddField("Alliance adverse:", data.Enemy.Name);
            }
            if (!string.IsNullOrWhiteSpace(data.Description))
            {
                embed.AddField("Note", data.Description);
            }
            embed.WithImageUrl(data.file.Url);

            var results = new List<Embed>()
            {
                embed.Build()
            };
            if (data.ScreenPrepa != null)
            {
                var prepaEmbed = new EmbedBuilder().WithImageUrl(data.ScreenPrepa.Url).WithColor(Color.Orange).WithCurrentTimestamp();
                results.Add(prepaEmbed.Build());
            }


            return results.ToArray();

        }
    }

}
