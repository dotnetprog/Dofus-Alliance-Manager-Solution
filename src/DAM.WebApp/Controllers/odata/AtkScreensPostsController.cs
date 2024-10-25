using Asp.Versioning;
using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Swagger.Odata.Annotations;
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
    public class AtkScreensPostsController : ODataController
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly AllianceContext _dbContext;

        public AtkScreensPostsController(IAllianceManagementServiceAsync allianceService,
        AllianceContext dbContextBuilder
        )
        {

            this._allianceService = allianceService;
            this._dbContext = dbContextBuilder;
        }

        [EnableQuery, HttpGet]
        [ProducesResponseType(typeof(OdataValueListAnnotation<AtkScreensPost>), 200)]
        public async Task<IQueryable<AtkScreensPost>> Get(ulong allianceid)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            //var dbCtx = await this._dbContextBuilder.CreateDbContextAsync();

            return _dbContext.AtkScreensPosts.Where(d => d.AllianceId == alliance.Id).AsQueryable();




        }
        [EnableQuery, HttpGet("{key}")]
        [ProducesResponseType(typeof(AtkScreensPost), 200)]
        public async Task<SingleResult<AtkScreensPost>> Get(ulong allianceid, Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var result = _dbContext.AtkScreensPosts.Where(d => d.AllianceId == alliance.Id && d.Id == key).AsQueryable();

            return SingleResult.Create<AtkScreensPost>(result);


        }



    }
}

