using DAM.Core.Abstractions.Services;
using DAM.WebApp.OAuth.Discord.Services;
using Discord.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DAM.WebApp.Filters.Action
{
    public class AllianceDataFilter : ActionFilterAttribute
    {
        private readonly IUserServiceAsync _userService;
        private readonly IDiscordBotService _discordBotService;
        public AllianceDataFilter(IUserServiceAsync userService, IDiscordBotService discordBotService)
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
                var controller = context.Controller as Controller;
                if (controller.ViewBag.Alliance?.Id != allianceid)
                {
                    bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                                                .Any(em => em.GetType() == typeof(AllowAnonymousAttribute)); //< -- Here it is

                    if (hasAllowAnonymous)
                    {
                        var guild = await _discordBotService.GetGuild(allianceid);
                        controller.ViewBag.Alliance = guild as RestGuild;
                        await next();
                        return;
                    }



                    var guilds = await _userService.GetGuilds();
                    if (!guilds.Any(g => g.Id == allianceid))
                    {


                        context.Result = new NotFoundResult();
                        return;

                    }
                    controller.ViewBag.Alliance = guilds.First(g => g.Id == allianceid);
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
