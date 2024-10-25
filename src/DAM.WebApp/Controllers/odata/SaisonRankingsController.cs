using Asp.Versioning;
using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands.Saisons;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Services;
using DAM.WebApp.Swagger.Odata.Annotations;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
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
    public class SaisonRankingsController : ODataController
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IAllianceMemberIdentityService _identityService;
        private readonly ISender _sender;
        private readonly IDAMMapper _mapper;
        private readonly AllianceContext _dbContext;

        public SaisonRankingsController(IAllianceManagementServiceAsync allianceService,
        AllianceContext dbContextBuilder,
        IAllianceMemberIdentityService identityService,
        IDAMMapper mapper,
        ISender sender)
        {

            this._allianceService = allianceService;
            this._dbContext = dbContextBuilder;
            _identityService = identityService;
            _mapper = mapper;
            _sender = sender;
        }

        [EnableQuery, HttpGet]
        [ProducesResponseType(typeof(OdataValueListAnnotation<SaisonRanking>), 200)]
        public async Task<IQueryable<SaisonRanking>> Get(ulong allianceid)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            return _dbContext.SaisonRankings
               .Join(_dbContext.Saisons, sr => sr.SaisonId, s => s.Id, (sr, s) => new { s = s, sr = sr })
               .Where(d => d.s.AllianceId == alliance.Id)
               .Select(d => d.sr).AsQueryable();



        }
        [HttpPatch]
        public async Task<SaisonRanking> Patch(ulong allianceid, [FromRoute] Guid key, Delta<SaisonRanking> saisonRanking)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var currentmember = await _identityService.GetCurrentMemberAsync();
            var saisonRankingDb = await _dbContext.SaisonRankings.FindAsync(key);

            if (saisonRankingDb == null)
            {
                throw new BadHttpRequestException("Aucune saison trouvé pour cet id = " + key.ToString());
            }
            saisonRanking.Patch(saisonRankingDb);

            ////develop mapping
            var command = _mapper.Map<UpdateSaisonRankingCommand, SaisonRanking>(saisonRankingDb);
            command.AllianceId = alliance.Id;
            command.ModifiedByUserId = currentmember.Id;
            ////develop command
            return await _sender.Send(command);
        }
        [EnableQuery, HttpGet("{key}")]
        [ProducesResponseType(typeof(Saison), 200)]
        public async Task<SingleResult<SaisonRanking>> Get(ulong allianceid, Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var result = _dbContext.SaisonRankings
                .Join(_dbContext.Saisons, (s) => s.SaisonId, (sr) => sr.Id, (sr, s) => new { s = s, sr = sr })
                .Where(d => d.s.AllianceId == alliance.Id && d.sr.Id == key)
                .Select(d => d.sr)
                .AsQueryable();

            return SingleResult.Create<SaisonRanking>(result);


        }
    }
}
