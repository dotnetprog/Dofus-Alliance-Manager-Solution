using DAM.Core.Abstractions.Services;
using Discord;
using Discord.Rest;

namespace DAM.Tool.DataMigrations.Services
{
    public class RestDiscordBotService : IDiscordBotService
    {
        private readonly DiscordRestClient _client;

        public RestDiscordBotService(DiscordRestClient client)
        {
            _client = client;
        }

        public async Task<IGuildChannel> GetChannel(ulong guildid, ulong channelid)
        {
            var guild = await _client.GetGuildAsync(guildid);
            return await guild.GetChannelAsync(channelid);
        }

        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id)
        {
            var guild = await _client.GetGuildAsync(id);
            return await guild.GetChannelsAsync();
        }

        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id, params ChannelType[] types)
        {
            var channels = await GetChannels(id);
            return channels.Where(c => types.Contains(c.GetChannelType().Value)).ToArray();
        }

        public async Task<IGuildUser> GetCurrentGuildUser(ulong guildid)
        {
            return await _client.GetCurrentUserGuildMemberAsync(guildid);
        }

        public async Task<IGuild> GetGuild(ulong guildid)
        {
            return await _client.GetGuildAsync(guildid);
        }

        public async Task<IReadOnlyCollection<IGuild?>> GetGuilds()
        {
            return await _client.GetGuildsAsync();
        }

        public async Task<IGuildUser> GetGuildUser(ulong guildid, ulong id)
        {
            return await _client.GetGuildUserAsync(guildid, id);
        }

        public async Task<IReadOnlyCollection<IRole?>> GetRoles(ulong guildid)
        {
            var guild = await _client.GetGuildAsync(guildid);
            return guild.Roles;
        }

        public async Task LeaveServer(ulong GuildId)
        {
            var guild = await _client.GetGuildAsync(GuildId);
            await guild.LeaveAsync();
        }

        public async Task React(ITextChannel channel, ulong MessageId, IEmote emote)
        {
            var message = await channel.GetMessageAsync(MessageId);
            if (message == null)
            {
                throw new ArgumentException($"No message id = {MessageId} in {channel.Name}", nameof(MessageId));
            }
            await message.AddReactionAsync(emote);
        }

        public Task<IUserMessage> SendMessage(ITextChannel channel, string content = null, Embed embed = null)
        {
            return channel.SendMessageAsync(text: content, embed: embed);
        }
    }
}
