using Asp.Versioning;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DAM.WebApp.Controllers.odata
{
    [ApiVersion("1")]
    [Route("odata/v{version:apiVersion}/{allianceid}/[controller]")]
    [ODataRouteComponent("odata/v1/{allianceid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TypeFilter(typeof(ApiUserAllianceAccess))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AllianceMembersController : ODataController
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly AllianceContext _dbContext;
        public AllianceMembersController(IAllianceManagementServiceAsync allianceService,
            AllianceContext dbContextBuilder
            )
        {

            this._allianceService = allianceService;
            this._dbContext = dbContextBuilder;
        }


        [EnableQuery, HttpGet]
        public async Task<IQueryable<AllianceMember>> Get(ulong allianceid)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            return _dbContext.Members.Where(m => m.AllianceId == alliance.Id).AsQueryable();
        }


    }
}
