using DAM.Core.Abstractions.Services;
using Discord;
using Discord.WebSocket;

namespace DAM.Bot.Services.Discord
{
    public class DiscordSocketService : IDiscordBotService
    {
        private readonly DiscordSocketClient _client;

        public DiscordSocketService(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task<IGuildChannel> GetChannel(ulong guildid, ulong channelid)
        {

            var guild = _client.GetGuild(guildid);

            return Task.FromResult((IGuildChannel)guild.GetChannel(channelid));
        }

        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id)
        {
            return _client.GetGuild(id)?.Channels;
        }

        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id, params ChannelType[] types)
        {
            var channels = await this.GetChannels(id);
            return channels.Where(c => c.GetChannelType().HasValue &&
                                      types.Contains(c.GetChannelType().Value)).ToList();
        }

        public Task<IGuildUser> GetCurrentGuildUser(ulong guildid)
        {
            var guild = _client.GetGuild(guildid);
            return Task.FromResult((IGuildUser)guild.GetUser(_client.CurrentUser.Id));
        }

        public Task<IGuild> GetGuild(ulong guildid)
        {
            return Task.FromResult((IGuild)_client.GetGuild(guildid));
        }

        public async Task<IReadOnlyCollection<IGuild?>> GetGuilds()
        {
            return _client.Guilds;
        }

        public async Task<IGuildUser> GetGuildUser(ulong guildid, ulong id)
        {
            var guild = _client.GetGuild(guildid);

            return guild.GetUser(id);
        }

        public async Task<IReadOnlyCollection<IRole?>> GetRoles(ulong guildid)
        {
            var guild = _client.GetGuild(guildid);
            return guild.Roles.ToList();
        }

        public async Task LeaveServer(ulong GuildId)
        {
            var guild = _client.GetGuild(GuildId);
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



        public async Task<IUserMessage> SendMessage(ITextChannel channel, string content = null, Embed embed = null)
        {
            return await channel.SendMessageAsync(text: content, embed: embed);
        }
    }
}
