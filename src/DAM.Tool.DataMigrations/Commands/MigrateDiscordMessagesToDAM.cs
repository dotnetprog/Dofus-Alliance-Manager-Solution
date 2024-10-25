
using Cocona;
using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.Domain.Enums;
using DAM.Domain.JsonData;
using DAM.Tool.DataMigrations.Constants;
using Discord;
using Discord.Rest;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DAM.Tool.DataMigrations.Migrations
{

    public class MigrateDiscordMessagesToDAM
    {
        private readonly DiscordRestClient _discordClient;
        private readonly ISender _sender;
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IScreenPostServiceAsync _screenPostService;
        private readonly IDbContextFactory<AllianceContext> _dbContextBuilder;




        public MigrateDiscordMessagesToDAM(DiscordRestClient discordClient, ISender sender, IAllianceManagementServiceAsync allianceManagementService, IScreenPostServiceAsync screenPostService, IDbContextFactory<AllianceContext> dbContextBuilder)
        {
            _discordClient = discordClient;
            _sender = sender;
            _allianceManagementService = allianceManagementService;
            _screenPostService = screenPostService;
            _dbContextBuilder = dbContextBuilder;
        }



        public Dictionary<ulong, ulong> GetMessageMapping(ulong serverid)
        {
            if (serverid == DiscordMigrationConsts.SPAM_HELLMINA_SERVER)
            {
                return DiscordMigrationConsts.SPAM_HELLMINA_ChannelMessageMapping;
            }
            throw new NotImplementedException();
        }

        [Command("Migrate")]
        public async Task Migrate([Option("dsid")] ulong DiscordServerid)
        {
            var alliance = await _allianceManagementService.GetAlliance(DiscordServerid.ToString());
            if (alliance.AllianceConfiguration.BotScreenBehaviorType != BotScreenBehaviorType.ChannelEnnemyCountMapping)
                return;

            var mapping = JsonConvert.DeserializeObject<AutoMappingChannelEnemyCountConfig>(alliance.AllianceConfiguration.BehaviorScreenConfigJSONData);
            var guild = await this._discordClient.GetGuildAsync(DiscordServerid);
            var guildMembers = await guild.GetUsersAsync().FlattenAsync();


            var configs = mapping.ToMappings();

            var messagesMapping = GetMessageMapping(DiscordServerid);


            using (var dbcontext = await this._dbContextBuilder.CreateDbContextAsync())
            {

                var allianceDefScreens = await (from def in dbcontext.DefScreensPosts
                                                join m in dbcontext.Members
                                                on def.CreatedByMemberId.Value equals m.Id
                                                where m.AllianceId == alliance.Id
                                                select def).ToArrayAsync();
                var allianceAtkScreens = await (from atk in dbcontext.AtkScreensPosts
                                                join m in dbcontext.Members
                                                on atk.CreatedByMemberId.Value equals m.Id
                                                where m.AllianceId == alliance.Id
                                                select atk).ToArrayAsync();
                foreach (var config in configs.Where(c => !string.IsNullOrWhiteSpace(c.channelid) && c.Type == ScreenPostType.Attack))
                {

                    var channelid = ulong.Parse(config.channelid);
                    if (!messagesMapping.ContainsKey(channelid))
                    {
                        continue;
                    }
                    await RunMigration(config, messagesMapping[channelid], allianceDefScreens, allianceAtkScreens, guildMembers.ToArray());

                }
            }








        }

        private async Task RunMigration(AutoMappingChannelEnemyCountData config, ulong startingMessageFrom,
            IReadOnlyCollection<DefScreensPost> existingDefs, IReadOnlyCollection<AtkScreensPost> existingAtks, IReadOnlyCollection<IGuildUser> guildMembers)
        {
            var channel = (await this._discordClient.GetChannelAsync(ulong.Parse(config.channelid))) as RestTextChannel;

            var allchannelmessages = (await GetMessagesAsync(channel, startingMessageFrom, Direction.Before)).ToList();

            var screenMessages = allchannelmessages.DistinctBy(c => c.Id).Where(IsScreenValid).ToList();

            List<IMessage> filteredScreens;
            if (config.Type == ScreenPostType.Defense)
            {
                filteredScreens = screenMessages.Where(s => !existingDefs.Any(d => d.DiscordMessageId == s.Id)).ToList();
            }
            else
            {
                filteredScreens = screenMessages.Where(s => !existingAtks.Any(d => d.DiscordMessageId == s.Id)).ToList();
            }

            foreach (var screen in filteredScreens)
            {
                var screenid = await CreateScreen(config, guildMembers, channel, screen);

                await screen.AddReactionAsync(new Emoji("📝"));


                var approvedCommand = new UpdateScreenPostValidationStatusCommand()
                {
                    ClosedByDiscordUserId = _discordClient.CurrentUser.Id.ToString(),
                    Screenkey = screenid.ToString(),
                    DiscordGuildId = channel.GuildId,
                    Status = ScreenValidationResultStatus.ManualyValid
                };
                await _sender.Send(approvedCommand);
                await screen.AddReactionAsync(new Emoji("✅"));


            }


            Console.WriteLine($"{channel.Name}, Count Screens : {screenMessages.Count}");

        }

        private ulong GetMemberIdOrReplaceItWithBotUser(IReadOnlyCollection<IGuildUser> guildMembers, ulong userid)
        {
            return guildMembers.Any(g => g.Id == userid) ? userid : _discordClient.CurrentUser.Id;
        }
        private IEnumerable<ulong> GetTaggedGuildMemberUserIds(IMessage screen, IReadOnlyCollection<IGuildUser> guildMembers)
        {
            var membersTagged = screen.Tags.Where(t => t.Type == TagType.UserMention).Select(t => (IUser)t.Value).ToList();

            foreach (var member in membersTagged)
            {
                var memberid = GetMemberIdOrReplaceItWithBotUser(guildMembers, member.Id);
                yield return memberid;

            }
        }
        private async Task<Guid> CreateScreen(AutoMappingChannelEnemyCountData config, IReadOnlyCollection<IGuildUser> guildMembers, RestTextChannel? channel, IMessage screen)
        {
            var createdby = GetMemberIdOrReplaceItWithBotUser(guildMembers, screen.Author.Id);



            var finalMembers = GetTaggedGuildMemberUserIds(screen, guildMembers);

            var command = new AddScreenPostCommand
            {
                Description = "Screen registered by data migration",
                DiscordGuildId = channel.GuildId,
                DiscordChannelId = channel.Id,
                Target = ScreenPostTarget.Perco,
                Type = config.Type.Value,
                DiscordMessageId = screen.Id,
                EnemyCount = config.EnemyCount.Value,
                ImageUrl = screen.Attachments.First().Url,
                OverridenCreatedon = screen.CreatedAt.UtcDateTime,
                OverridenCreatedByMemberId = createdby,
                AllianceMembers = finalMembers.ToArray(),
            };
            if (screen.Attachments.Count > 1)
            {
                command.ImagePrepUrl = screen.Attachments.ElementAt(1).Url;
            }
            return await _sender.Send(command);
        }

        private bool IsScreenValid(IMessage message)
        {
            return message.Tags.Any(t => t.Type == TagType.UserMention) && message.Attachments.Any();
        }

        private async Task<IEnumerable<IMessage>> GetMessagesAsync(ITextChannel channel, ulong startingmessageid, Direction direction)
        {
            var results = new List<IMessage>();

            var messages = await channel.GetMessagesAsync(startingmessageid, direction, 100).FlattenAsync();
            messages = messages.OrderBy(m => m.CreatedAt);
            if (messages.Any())
            {
                results.AddRange(messages);
            }
            if (messages.Count() == 100)
            {
                results.AddRange(await GetMessagesAsync(channel, direction == Direction.Before ? messages.First().Id : messages.Last().Id, direction));
            }

            return results;


        }


    }
}
