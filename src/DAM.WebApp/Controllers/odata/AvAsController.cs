using Asp.Versioning;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DAM.WebApp.Controllers.odata
{
    [ApiVersion("1")]
    [Route("odata/v{version:apiVersion}/{allianceid}/[controller]")]
    [ODataRouteComponent("odata/v1/{allianceid}")]

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TypeFilter(typeof(ApiUserAllianceAccess))]
    public class AvAsController : ODataController
    {

        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly AllianceContext _dbContext;
        public AvAsController(IAllianceManagementServiceAsync allianceService,
            AllianceContext dbContextBuilder
            )
        {

            this._allianceService = allianceService;
            this._dbContext = dbContextBuilder;
        }

        [EnableQuery, HttpGet]
        public async Task<IQueryable<AvA>> Get(ulong allianceid)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            //var dbCtx = await this._dbContextBuilder.CreateDbContextAsync();

            return _dbContext.AvA.Where(d => d.AllianceId == alliance.Id)
                .AsQueryable();


        }

        [EnableQuery, HttpGet("{key}")]
        public async Task<SingleResult<AvA>> Get(ulong allianceid, Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var result = _dbContext.AvA.Where(d => d.AllianceId == alliance.Id && d.Id == key)
                .AsQueryable();

            return SingleResult.Create<AvA>(result);


        }
        [HttpDelete]
        public async Task Delete(ulong allianceid, [FromRoute] Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var ava = this._dbContext.AvA.SingleOrDefault(d => d.Id == key);

            if (ava != null)
            {
                this._dbContext.AvA.Remove(ava);
            }

            await this._dbContext.SaveChangesAsync();

        }
    }
}
