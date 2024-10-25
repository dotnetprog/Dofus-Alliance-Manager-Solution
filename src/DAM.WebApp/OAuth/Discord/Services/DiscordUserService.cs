using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Authentication;

namespace DAM.WebApp.OAuth.Discord.Services
{
    public interface IUserServiceAsync
    {
        Task<IReadOnlyCollection<RestUserGuild>> GetGuilds();
        Task<RestUserGuild?> GetGuidById(ulong id);

        Task<RestGuildUser> GetCurrentUserGuildMemberAsync(ulong guildid);

        // Task<IReadOnlyCollection<RestGuildChannel>> GetChannels(ulong id);
    }
    public class DiscordUserService : IUserServiceAsync
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<Task<DiscordRestClient>> _Client;
        public DiscordUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;


            this._Client = new Lazy<Task<DiscordRestClient>>(async () =>
            {
                var client = new DiscordRestClient();
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("Discord", "access_token");
                await client.LoginAsync(TokenType.Bearer, token);
                return client;
            });






        }

        public async Task<RestGuildUser> GetCurrentUserGuildMemberAsync(ulong guildid)
        {
            var client = await _Client.Value;

            var guilds = await GetGuilds();
            var guild = guilds.FirstOrDefault(x => x.Id == guildid);
            return await guild.GetCurrentUserGuildMemberAsync();
        }

        //public async Task<IReadOnlyCollection<RestGuildChannel>> GetChannels(ulong id)
        //{
        //    var client = await _Client.Value;
        //    var guild = await client.GetGuildAsync(id);

        //    var channels = await guild.GetChannelsAsync();

        //    var textChannels = channels.Where(c => c.GetChannelType() == ChannelType.Text);
        //    return textChannels.ToList();

        //}




        public async Task<RestUserGuild?> GetGuidById(ulong id)
        {
            var client = await _Client.Value;

            var guilds = await GetGuilds();
            var guild = guilds.FirstOrDefault(x => x.Id == id);



            if (guild?.Permissions.ManageGuild ?? false)
                return guild;

            return null;
        }

        public async Task<IReadOnlyCollection<RestUserGuild>> GetGuilds()
        {
            var client = await _Client.Value;

            var guilds = await client.GetGuildSummariesAsync().FlattenAsync();

            var managedGuilds = guilds.Where(g => g.Permissions.Administrator || client.CurrentUser.Id == 309480835189571584);
            return managedGuilds.ToArray();


        }
    }
}
