using Asp.Versioning;
using DAM.Core.Abstractions.Identity;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Ïdentity;
using DAM.WebApp.Common.Helper.Security;
using DAM.WebApp.Filters.Action;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DAM.WebApp.Controllers.odata
{
    [ApiVersion("1")]
    [Route("odata/v{version:apiVersion}/{allianceid}/[controller]")]
    [ODataRouteComponent("odata/v1/{allianceid}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TypeFilter(typeof(ApiUserAllianceAccess))]
    public class AppUsersController : ODataController
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IAppUserIdentityManagerAsync AppIdentityManager;


        public AppUsersController(IAppUserIdentityManagerAsync appIdentityManager, IAllianceManagementServiceAsync allianceService)
        {
            this.AppIdentityManager = appIdentityManager;
            _allianceService = allianceService;

        }

        [EnableQuery, HttpGet]
        public async Task<IQueryable<AppUser>> Get(ulong allianceid)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            //var dbCtx = await this._dbContextBuilder.CreateDbContextAsync();
            var results = await AppIdentityManager.GetAllusers(alliance.Id);

            return results.AsQueryable();
        }
        [EnableQuery, HttpGet("{key}")]
        public async Task<SingleResult<AppUser>> Get(ulong allianceid, Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");
            }

            var users = await AppIdentityManager.GetAllusers(alliance.Id);
            var result = users.Where(u => u.Id == key).AsQueryable();

            return SingleResult.Create<AppUser>(result);


        }
        [HttpPost]
        public async Task<string> SetClientSecret(ulong allianceid, [FromODataUri] Guid key)
        {

            //Generate a randomstring 
            var secret = PasswordGenerator.Generate(32, 7, true, true, true, true);
            //hash it and store the hash
            await this.AppIdentityManager.UpdateClientSecret(key, secret);

            return secret;
        }
        [HttpDelete]
        public async Task Delete(ulong allianceid, [FromRoute] Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            await this.AppIdentityManager.DeleteAsync(key);

        }


    }
}
