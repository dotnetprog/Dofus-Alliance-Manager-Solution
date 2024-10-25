using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DAM.WebApp.Filters.Action
{
    public class CheckAlliianceRegistration : ActionFilterAttribute
    {

        private readonly IAllianceManagementServiceAsync _allianceService;

        public CheckAlliianceRegistration(IAllianceManagementServiceAsync allianceService) { 
        
            this._allianceService = allianceService;
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
                var alliance = await this._allianceService.GetAlliance(allianceid.ToString());
                if (alliance == null)
                {
                    context.Result = new BadRequestObjectResult(new { Message = "DAMBot n'a pas été invité à ce serveur discord." });
                    return;
                }
                var controller = context.Controller as Controller;
                controller.ViewBag.allianceContext = alliance;
            }

            await next();
        }
    }
}
