using Asp.Versioning;
using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands;
using DAM.Core.Requests.Commands.Saisons;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.Saison;
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
    public class SaisonsController : ODataController
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly AllianceContext _dbContext;
        private readonly ISaisonServiceAsync _saisonService;
        private readonly IDAMMapper _mapper;
        private readonly ISender _sender;

        public SaisonsController(IAllianceManagementServiceAsync allianceService,
        AllianceContext dbContextBuilder,
        ISaisonServiceAsync saisonService,
        IDAMMapper mapper,
        ISender sender
        )
        {
            this._saisonService = saisonService;
            this._allianceService = allianceService;
            this._dbContext = dbContextBuilder;
            this._mapper = mapper;
            this._sender = sender;
        }

        [EnableQuery, HttpGet]
        [ProducesResponseType(typeof(OdataValueListAnnotation<Saison>), 200)]
        public async Task<IQueryable<Saison>> Get(ulong allianceid)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            //var dbCtx = await this._dbContextBuilder.CreateDbContextAsync();

            return _dbContext.Saisons.Where(d => d.AllianceId == alliance.Id)
                .AsQueryable();


        }
        [EnableQuery, HttpGet("{key}")]
        [ProducesResponseType(typeof(Saison), 200)]
        public async Task<SingleResult<Saison>> Get(ulong allianceid, Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var result = _dbContext.Saisons
                .Where(s => s.AllianceId == alliance.Id && s.Id == key)
                .AsQueryable();

            return SingleResult.Create<Saison>(result);


        }
        [HttpPatch]
        public async Task<Saison> Patch(ulong allianceid, [FromRoute] Guid key, Delta<Saison> saison)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var saisonDb = await _dbContext.Saisons.FindAsync(key);

            if (saisonDb == null)
            {
                throw new BadHttpRequestException("Aucune saison trouvé pour cet id = " + key.ToString());
            }
            saison.Patch(saisonDb);
            var userCurrentDiscordId = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == "discordId")?.Value ?? string.Empty);
            var command = _mapper.Map<UpdateSaisonCommand, Saison>(saisonDb);
            command.AllianceId = alliance.Id;
            command.AllowOverlap = alliance.AllianceConfiguration.AllowSeasonOverlap ?? false;
            command.ModifiedByDiscordId = userCurrentDiscordId;

            await _sender.Send(command);
            saisonDb = await _dbContext.Saisons.FindAsync(key);
            return saisonDb;


        }
        [HttpDelete]
        public async Task Delete(ulong allianceid, [FromRoute] Guid key)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            await this._saisonService.Delete(key);

        }
        [HttpPost]
        public async Task GenerateRankings(ulong allianceid, [FromRoute] Guid key, [FromBody] GenerateSaisonRankingsViewModel request)
        {
            var userCurrentDiscordId = ulong.Parse(request?.DiscordUserCallerId ?? User.Claims.FirstOrDefault(c => c.Type == "discordId")?.Value ?? string.Empty);
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var command = new GenerateSaisonRankingsCommand()
            {
                AllianceId = alliance.Id,
                CreatedByUserId = userCurrentDiscordId,
                SaisonId = key
            };

            await _sender.Send(command);
        }
        [HttpPost]
        public async Task PublishRanking(ulong allianceid, [FromRoute] Guid key)
        {

            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var command = new PublishSaisonRankingsCommand()
            {
                GuidServerId = allianceid,
                SaisonId = key
            };

            await _sender.Send(command);
        }
    }
}
