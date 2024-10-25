using DAM.Core.Abstractions.Services;
using DAM.Database.Contexts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.AvA;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAM.WebApp.Controllers.Api
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TypeFilter(typeof(ApiUserAllianceAccess))]
    [Route("api/[controller]/{allianceid}/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AvaController : ControllerBase
    {
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IAvAService _avaService;
        public AvaController(IAllianceManagementServiceAsync allianceService,
            AllianceContext dbContextBuilder,
            IAvAService avaService
            )
        {

            this._allianceService = allianceService;
            this._avaService = avaService;
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task SetMemberValidationState(ulong allianceid, Guid Id, [FromBody] SetAvaMemberStateViewModel payload)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            var userDiscordId = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == "discordId").Value);
            var currentUser = await _allianceService.GetMember(alliance.Id, userDiscordId.ToString());

            var AvAMember = await _avaService.GetMember(Id);
            if (AvAMember.AvA.AllianceId != alliance.Id)
            {
                throw new BadHttpRequestException($"MemberId = {Id.ToString()} does not exist for alliance {alliance.Alias}");
            }

            var avaMemberToUpdate = new AvaMember
            {
                Id = Id,
                ValidationState = payload.State,
                ValidatedById = currentUser.Id,

            };
            await _avaService.EditAvAMember(avaMemberToUpdate);


        }
        [HttpPut]
        [Route("{Id}")]
        public async Task Distribute(ulong allianceid, Guid Id)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            var userDiscordId = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == "discordId").Value);
            var currentUser = await _allianceService.GetMember(alliance.Id, userDiscordId.ToString());

            var ava = await _avaService.GetAvA(alliance.Id, Id, true);
            if (ava == null)
            {
                throw new BadHttpRequestException($"AvA with Id = {Id.ToString()} does not exist for alliance {alliance.Alias}");
            }
            if (ava.State == AvaState.Closed || !ava.ResultState.HasValue)
            {
                throw new BadHttpRequestException("AvA do not respect criterias.");

            }
            var participants = ava.Members.Where(m => m.ValidationState != AvAValidationState.Rejected).ToList();
            var nbParticipants = participants.Count;

            var IsFixe = ava.MontantPayeFixe.HasValue;
            var parjoueur = 0;
            if (ava.ResultState.HasValue)
            {
                if (!IsFixe)
                {
                    var percentage = (ava.PourcentagePaye.Value / 100);
                    var totalRedistrib = ava.MontantPepitesObtenu.HasValue ? Math.Round(ava.MontantPepitesObtenu.Value * percentage, 0, MidpointRounding.ToNegativeInfinity) : 0;
                    parjoueur = nbParticipants > 0 && totalRedistrib > 0 ? Convert.ToInt32(Math.Round(totalRedistrib / nbParticipants, 0, MidpointRounding.ToNegativeInfinity)) : 0;

                }
                else
                {
                    parjoueur = ava.MontantPayeFixe.Value;

                }
            }
            await _avaService.PayMembers(ava.Id, parjoueur);


        }
        [HttpPut]
        [Route("{Id}")]
        public async Task Close(ulong allianceid, Guid Id)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            var userDiscordId = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == "discordId").Value);
            var currentUser = await _allianceService.GetMember(alliance.Id, userDiscordId.ToString());

            var ava = await _avaService.GetAvA(alliance.Id, Id, false);
            if (ava == null)
            {
                throw new BadHttpRequestException($"AvA with Id = {Id.ToString()} does not exist for alliance {alliance.Alias}");
            }
            if (ava.State == AvaState.Closed || !ava.ResultState.HasValue)
            {
                throw new BadHttpRequestException("AvA do not respect criterias.");

            }


            ava.ClosedById = currentUser.Id;
            ava.State = AvaState.Closed;
            await _avaService.UpdateAva(ava);


        }
        [HttpPut]
        [Route("{Id}")]
        public async Task Open(ulong allianceid, Guid Id)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());

            var userDiscordId = ulong.Parse(User.Claims.FirstOrDefault(c => c.Type == "discordId").Value);
            var currentUser = await _allianceService.GetMember(alliance.Id, userDiscordId.ToString());

            var ava = await _avaService.GetAvA(alliance.Id, Id, false);
            if (ava == null)
            {
                throw new BadHttpRequestException($"AvA with Id = {Id.ToString()} does not exist for alliance {alliance.Alias}");
            }
            if (ava.State != AvaState.Closed)
            {
                throw new BadHttpRequestException("AvA do not respect criterias.");

            }


            ava.ClosedById = currentUser.Id;
            ava.State = AvaState.Open;
            await _avaService.UpdateAva(ava);


        }




    }
}
