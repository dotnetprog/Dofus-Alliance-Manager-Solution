using DAM.Core.Abstractions.Services;
using DAM.WebApp.OAuth.Discord.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DAM.WebApp.Filters.Action
{
    public class ApiUserAllianceAccess : ActionFilterAttribute
    {
        private readonly IUserServiceAsync _userService;
        private readonly IDiscordBotService _discordBotService;
        public ApiUserAllianceAccess(IUserServiceAsync userService, IDiscordBotService discordBotService)
        {
            this._userService = userService;
            _discordBotService = discordBotService;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.RouteData.Values.ContainsKey("allianceid"))
            {
                await next();
                return;
            }






            if (ulong.TryParse(context.RouteData.Values["allianceid"]?.ToString(), out ulong allianceid))
            {

                bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                                                .Any(em => em.GetType() == typeof(AllowAnonymousAttribute)); //< -- Here it is

                if (hasAllowAnonymous)
                {
                    var guild = await _discordBotService.GetGuild(allianceid);
                    if (guild == null)
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }
                    await next();
                    return;
                }


                var jwttype = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "type");
                if (jwttype?.Value == "oauth")
                {
                    var jwtallianceid = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "alliancediscordid");
                    if (jwtallianceid.Value != allianceid.ToString())
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }
                }
                else
                {

                    var guilds = await _userService.GetGuilds();
                    if (!guilds.Any(g => g.Id == allianceid))
                    {
                        context.Result = new NotFoundResult();
                        return;
                    }
                }

            }
            else
            {
                context.Result = new NotFoundResult();
                return;
            }
            await next();
        }


    }
}
