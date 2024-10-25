using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;

namespace DAM.WebApp.Services
{
    public class HttpContextAllianceMemberIdentityService : IAllianceMemberIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        private readonly IDiscordBotService _botService;
        const string ALLIANCE_ROUTE_KEY = "allianceid";
        const string HEADER_DISCORD_USER = "x-discorduserid-x";
        public HttpContextAllianceMemberIdentityService(IHttpContextAccessor httpContextAccessor,
            IAllianceManagementServiceAsync allianceManagementServiceAsync,
            IDiscordBotService botService)
        {
            _httpContextAccessor = httpContextAccessor;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _botService = botService;
        }
        public Task<AllianceMember> GetCurrentMemberAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            var User = _httpContextAccessor.HttpContext.User;
            var routeData = context.GetRouteData();

            if (!routeData.Values.ContainsKey(ALLIANCE_ROUTE_KEY))
            {
                return null;
            }
            var allianceGuildId = ulong.Parse(routeData.Values[ALLIANCE_ROUTE_KEY]?.ToString());
            var contextDiscordUserId = User.Claims.FirstOrDefault(c => c.Type == "discordId")?.Value ?? string.Empty;
            var userCurrentDiscordId = string.IsNullOrEmpty(contextDiscordUserId) ? (ulong?)null : ulong.Parse(contextDiscordUserId);
            if (!userCurrentDiscordId.HasValue && context.Request.Headers.TryGetValue(HEADER_DISCORD_USER, out var headerdiscorduserid))
            {
                userCurrentDiscordId = ulong.Parse(headerdiscorduserid.FirstOrDefault());
            }
            if (!userCurrentDiscordId.HasValue)
            {
                throw new BadHttpRequestException($"No discorduserid was resolvable. Ensure the bot has enough rights in your server or {HEADER_DISCORD_USER} headers is present.");
            }


            return GetOrCreateMember(allianceGuildId, userCurrentDiscordId.Value);


        }
        private async Task<AllianceMember> GetOrCreateMember(ulong GuildId, ulong discordid)
        {
            var alliance = await _allianceManagementServiceAsync.GetAlliance(GuildId.ToString());
            if (alliance == null)
            {
                return null;
            }
            var discorduser = await _botService.GetGuildUser(GuildId, discordid);
            if (discorduser == null)
            {
                throw new InvalidDiscordUserException(discordid, GuildId);
            }
            var member = await this._allianceManagementServiceAsync.GetOrCreateAllianceMember(alliance.Id, discorduser);
            return member;
        }
    }
}
