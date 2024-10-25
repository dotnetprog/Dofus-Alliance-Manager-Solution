using DAM.Core.Abstractions.Services;
using Discord;
using Discord.Rest;

namespace DAM.WebApp.Services
{

    public class DefaultBarnakUser : IGuildUser
    {
        private ulong _guildid { get; set; }

        public DefaultBarnakUser(ulong GuildId)
        {
            this._guildid = GuildId;
        }



        public DateTimeOffset? JoinedAt => throw new NotImplementedException();

        public string DisplayName => "Barnak";

        public string Nickname => "Barnak";

        public string DisplayAvatarId => throw new NotImplementedException();

        public string GuildAvatarId => throw new NotImplementedException();

        public GuildPermissions GuildPermissions => throw new NotImplementedException();

        public IGuild Guild => throw new NotImplementedException();

        public ulong GuildId => _guildid;

        public DateTimeOffset? PremiumSince => throw new NotImplementedException();

        public IReadOnlyCollection<ulong> RoleIds => throw new NotImplementedException();

        public bool? IsPending => throw new NotImplementedException();

        public int Hierarchy => throw new NotImplementedException();

        public DateTimeOffset? TimedOutUntil => throw new NotImplementedException();

        public GuildUserFlags Flags => throw new NotImplementedException();

        public string AvatarId => throw new NotImplementedException();

        public string Discriminator => throw new NotImplementedException();

        public ushort DiscriminatorValue => throw new NotImplementedException();

        public bool IsBot => throw new NotImplementedException();

        public bool IsWebhook => throw new NotImplementedException();

        public string Username => "_barnak_";

        public UserProperties? PublicFlags => throw new NotImplementedException();

        public DateTimeOffset CreatedAt => throw new NotImplementedException();

        public ulong Id => 309480835189571584;

        public string Mention => throw new NotImplementedException();

        public UserStatus Status => throw new NotImplementedException();

        public IReadOnlyCollection<ClientType> ActiveClients => throw new NotImplementedException();

        public IReadOnlyCollection<IActivity> Activities => throw new NotImplementedException();

        public bool IsDeafened => throw new NotImplementedException();

        public bool IsMuted => throw new NotImplementedException();

        public bool IsSelfDeafened => throw new NotImplementedException();

        public bool IsSelfMuted => throw new NotImplementedException();

        public bool IsSuppressed => throw new NotImplementedException();

        public IVoiceChannel VoiceChannel => throw new NotImplementedException();

        public string VoiceSessionId => throw new NotImplementedException();

        public bool IsStreaming => throw new NotImplementedException();

        public bool IsVideoing => throw new NotImplementedException();

        public DateTimeOffset? RequestToSpeakTimestamp => throw new NotImplementedException();

        public string GlobalName => throw new NotImplementedException();

        public string AvatarDecorationHash => throw new NotImplementedException();

        public ulong? AvatarDecorationSkuId => throw new NotImplementedException();

        public Task AddRoleAsync(ulong roleId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRoleAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return "https://cdn.discordapp.com/embed/avatars/2.png";
        }

        public string GetDefaultAvatarUrl()
        {
            return "https://cdn.discordapp.com/embed/avatars/2.png";
        }

        public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return "https://cdn.discordapp.com/embed/avatars/2.png";
        }

        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return "https://cdn.discordapp.com/embed/avatars/2.png";
        }

        public ChannelPermissions GetPermissions(IGuildChannel channel)
        {
            throw new NotImplementedException();
        }

        public Task KickAsync(string reason = null, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoleAsync(ulong roleId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoleAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTimeOutAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task SetTimeOutAsync(TimeSpan span, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string GetAvatarDecorationUrl()
        {
            throw new NotImplementedException();
        }
    }

    public class DiscordBotService : IDiscordBotService
    {
        private readonly Lazy<Task<DiscordRestClient>> _Client;


        public DiscordBotService(string token)
        {

            this._Client = new Lazy<Task<DiscordRestClient>>(async () =>
            {
                var client = new DiscordRestClient();
                await client.LoginAsync(TokenType.Bot, token);
                return client;
            });
        }

        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id)
        {
            var client = await _Client.Value;
            var guild = await client.GetGuildAsync(id);
            if (guild == null)
                return new List<RestGuildChannel>();
            var channels = await guild.GetTextChannelsAsync();

            return channels.ToList();

        }
        public async Task<IReadOnlyCollection<IGuildChannel?>> GetChannels(ulong id, params ChannelType[] types)
        {
            var client = await _Client.Value;
            var guild = await client.GetGuildAsync(id);
            if (guild == null)
                return new List<RestGuildChannel>();
            var channels = await guild.GetChannelsAsync();


            return channels.Where(c => c.GetChannelType().HasValue && types.Contains(c.GetChannelType().Value)).ToList();

        }
        public async Task LeaveServer(ulong GuildId)
        {
            var client = await _Client.Value;
            var guild = await client.GetGuildAsync(GuildId);
            await guild.LeaveAsync();
        }
        public async Task<IReadOnlyCollection<IRole?>> GetRoles(ulong guildid)
        {
            var client = await _Client.Value;
            var guild = await client.GetGuildAsync(guildid);
            if (guild == null)
                return new List<RestRole>();
            return guild.Roles.Where(r => !r.IsManaged).ToArray();
        }

        public async Task<IReadOnlyCollection<IGuild?>> GetGuilds()
        {
            var client = await _Client.Value;
            var guilds = await client.GetGuildsAsync();
            return guilds;

        }

        public async Task<IGuildUser> GetGuildUser(ulong guildid, ulong id)
        {
            var client = await _Client.Value;
            try
            {
                var user = await client.GetGuildUserAsync(guildid, id);
                if (user == null)
                {
                    throw new Exception("cannot find user");
                }
                return user;
            }
            catch
            {
                var barnak = new DefaultBarnakUser(guildid);
                if (id == barnak.Id)
                {
                    return barnak;
                }
                throw;
            }

        }

        public Task<IUserMessage> SendMessage(ITextChannel channel, string content = null, Embed embed = null)
        {
            return channel.SendMessageAsync(text: content, embed: embed);
        }

        public async Task<IGuildChannel> GetChannel(ulong guildid, ulong channelid)
        {
            var client = await _Client.Value;

            var guild = await client.GetGuildAsync(guildid);
            return await guild.GetChannelAsync(channelid);

        }

        public async Task<IGuildUser> GetCurrentGuildUser(ulong guildid)
        {
            var client = await _Client.Value;
            return await client.GetCurrentUserGuildMemberAsync(guildid);
        }

        public async Task<IGuild> GetGuild(ulong guildid)
        {
            var client = await _Client.Value;
            return await client.GetGuildAsync(guildid);
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
    }
}
