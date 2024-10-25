using Cocona;
using DAM.Core.Abstractions.Services;
using DAM.Core.Helpers.Discord;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using Discord;
using Discord.Rest;
using Microsoft.EntityFrameworkCore;

namespace DAM.Tool.DataMigrations.Commands
{
    public class SPAMCustomRankingRowFormat : IRankingRowFormat
    {
        private static string[] podium = new string[] { "🏆", "🥈", "🥉" };
        public string Format(SaisonRanking ranking, int index)
        {
            var rankEmote = podium.Length < (index + 1) ? $"#{(index + 1).ToString("D3")}" : podium[index];

            var activitiesCount = ranking.Nombre_defense + ranking.Nombre_attaques;

            return $"{rankEmote} **{ranking.Member.Nickname ?? ranking.Member.Alias}** : {activitiesCount} points (def:{ranking.Nombre_defense},atk:{ranking.Nombre_attaques}) {Environment.NewLine}";
        }
    }
    public class CustomLadderCommands
    {
        private readonly DiscordRestClient _discordClient;
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly ISaisonServiceAsync _saisonServiceAsync;
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;




        public CustomLadderCommands(DiscordRestClient discordClient, IAllianceManagementServiceAsync allianceManagementService, ISaisonServiceAsync saisonServiceAsync, IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _discordClient = discordClient;
            _allianceManagementService = allianceManagementService;
            _saisonServiceAsync = saisonServiceAsync;
            _dbContextBuilder = dbContextBuilder;
        }



        [Command("Ladder")]
        public async Task PublishCustomerLadder([Option("dsid")] ulong discordserverid, [Option("cid")] ulong channelid, [Option("sid")] Guid SaisonId)
        {

            var alliance = await _allianceManagementService.GetAlliance(discordserverid.ToString());

            var saison = await _saisonServiceAsync.GetById(alliance.Id, SaisonId);
            var guild = await _discordClient.GetGuildAsync(discordserverid);
            var rankings = await _saisonServiceAsync.GetRankings(alliance.Id, saison.Id, true);
            var members = await guild.GetUsersAsync().FlattenAsync();
            foreach (var ranking in rankings)
            {
                var guildmember = members.FirstOrDefault(m => m.Id.ToString() == ranking.Member.DiscordId);
                if (guildmember == null)
                {
                    continue;
                }
                ranking.Member.Nickname = guildmember.Nickname ?? guildmember.DisplayName ?? guildmember.Username;
            }




            var channel = (ITextChannel)(await guild.GetChannelAsync(channelid));


            var embeds = EmbedHelper.BuildLadderEmbed(guild.Name, guild.IconUrl ?? string.Empty, saison, rankings.OrderByDescending(d => (d.Nombre_attaques + d.Nombre_defense)).ToArray(), new SPAMCustomRankingRowFormat(), false);

            var messages = await channel.GetMessagesAsync().FlattenAsync();
            if (messages.Any())
            {
                await channel.DeleteMessagesAsync(messages);
            }
            foreach (var embed in embeds)
            {
                await channel.SendMessageAsync(embed: embed);
            }

        }
        [Command("Inactives")]//Inactives --dsid 1128083543462006844 --cid 1128094390741631157
        public async Task PublishInactiveLadder([Option("dsid")] ulong discordserverid, [Option("cid")] ulong channelid)
        {

            var alliance = await _allianceManagementService.GetAlliance(discordserverid.ToString());
            var Dbmembers = await _allianceManagementService.GetAllMembers(alliance.Id);
            var guild = await _discordClient.GetGuildAsync(discordserverid);
            var members = await guild.GetUsersAsync().FlattenAsync();

            var membersThatArenotInDb = members.Where(m => !m.IsBot && !m.IsWebhook && !Dbmembers.Any(dm => dm.DiscordId == m.Id.ToString())).ToArray();




            var channel = (ITextChannel)(await guild.GetChannelAsync(channelid));


            var embeds = EmbedHelper.BuildPlayerList(guild.Name, guild.IconUrl ?? string.Empty, "Liste des joueurs inactifs", membersThatArenotInDb);


            foreach (var embed in embeds)
            {
                await channel.SendMessageAsync(embed: embed);
            }

        }
    }
}
