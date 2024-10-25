using DAM.Core.Abstractions.Services;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.Caching.Memory;

namespace DAM.WebApp.Services
{
    public class CachedDiscordBotService : IDiscordBotService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDiscordBotService _botService;
        private readonly TimeSpan _defaultDelay;
        public CachedDiscordBotService(IMemoryCache memoryCache, IDiscordBotService botService)
        {
            this._memoryCache = memoryCache;
            _botService = botService;
            _defaultDelay = TimeSpan.FromMinutes(15);
        }

        public async Task<IGuildChannel> GetChannel(ulong guildid, ulong channelid)
        {
            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetChannel.{guildid}.{channelid}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetChannel(guildid, channelid);

            });
            return results;
        }

        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id)
        {
            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetChannels.{id}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetChannels(id);

            });
            return results ?? new List<RestGuildChannel>();
        }
        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id, params ChannelType[] types)
        {
            var subkey = string.Join('|', types.Select(t => t.ToString()).Order());
            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetChannels.{id}{subkey}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetChannels(id, types);

            });
            return results ?? new List<IGuildChannel>();
        }

        public async Task<IGuildUser> GetCurrentGuildUser(ulong guildid)
        {
            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetCurrentGuildUser.{guildid}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetCurrentGuildUser(guildid);

            });
            return results;
        }

        public async Task<IGuild> GetGuild(ulong guildid)
        {
            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetGuild.{guildid}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetGuild(guildid);

            });
            return results;
        }

        public async Task<IReadOnlyCollection<IGuild?>> GetGuilds()
        {
            var subkey = "Guilds";
            var results = await this._memoryCache.GetOrCreateAsync(subkey, async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetGuilds();

            });
            return results;
        }

        public async Task<IGuildUser> GetGuildUser(ulong guildid, ulong id)
        {
            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetGuildUser.{guildid}.{id}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetGuildUser(guildid, id);

            });
            return results;
        }

        public async Task<IReadOnlyCollection<IRole?>> GetRoles(ulong guildid)
        {

            var results = await this._memoryCache.GetOrCreateAsync($"bot.GetRoles.{guildid}", async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = _defaultDelay;

                return await _botService.GetRoles(guildid);

            });
            return results ?? new List<RestRole>();
        }

        public async Task LeaveServer(ulong GuildId)
        {
            await this._botService.LeaveServer(GuildId);
        }

        public async Task React(ITextChannel channel, ulong MessageId, IEmote emote)
        {
            await this._botService.React(channel, MessageId, emote);
        }

        public Task<IUserMessage> SendMessage(ITextChannel channel, string content = null, Embed embed = null)
        {
            return SendMessage(channel, content, embed);
        }
    }
}
