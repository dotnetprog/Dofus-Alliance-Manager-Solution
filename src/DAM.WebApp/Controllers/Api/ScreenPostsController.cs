using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Commands;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Core.Requests.Queries.Entities;
using DAM.Core.Requests.Queries.ScreenPosts;
using DAM.Domain.Entities;
using DAM.WebApp.Filters.Action;
using DAM.WebApp.Models.Api.ScreenPosts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAM.WebApp.Controllers.Api
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [TypeFilter(typeof(ApiUserAllianceAccess))]
    [Route("api/[controller]/{allianceid}/[action]")]
    public class ScreenPostsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IDAMMapper _mapper;
        private readonly IAllianceManagementServiceAsync _allianceService;
        public ScreenPostsController(ISender sender, IDAMMapper mapper, IAllianceManagementServiceAsync allianceService)
        {
            _sender = sender;
            _mapper = mapper;
            _allianceService = allianceService;
        }


        [AllowAnonymous]
        [HttpGet("{SeasonRankingId}")]
        public async Task<IReadOnlyCollection<GetScreenPostQueryResult>> GetScreensBySeasonRankingId(ulong allianceid, Guid SeasonRankingId)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var Query = new GetScreensBySeasonRankingIdQuery
            {
                AllianceId = alliance.Id,
                SeasonRankingId = SeasonRankingId
            };
            var results = await _sender.Send(Query);
            return results;

        }

        [HttpPost]
        public async Task<Guid> CreateScreen(ulong allianceid, [FromBody] CreateScreenPostRequest request, CancellationToken cancellationToken)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var command = _mapper.Map<AddScreenPostCommand, CreateScreenPostRequest>(request);
            command.DiscordGuildId = allianceid;
            var id = await _sender.Send(command, cancellationToken);
            return id;
        }
        [HttpPatch("{discordmessageid}")]
        public async Task UpdateScreen(ulong allianceid, string discordmessageid, [FromBody] UpdateScreenPostRequest request, CancellationToken cancellationToken)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var command = _mapper.Map<UpdateScreenPostCommand, UpdateScreenPostRequest>(request);

            if (ulong.TryParse(discordmessageid, out ulong messageid))
            {
                command.DiscordMessageId = messageid;
            }
            else
            {
                throw new ValidationException(new List<ValidationFailure>()
                {
                    new ValidationFailure("discordmessageid","Cannot be parsed as ulong (bigint)")
                });
            }


            command.DiscordGuildId = allianceid;
            await _sender.Send(command, cancellationToken);
        }
        [HttpPatch("{screenkey}")]
        public async Task Approve(ulong allianceid, string screenkey, [FromBody] CloseScreenPostRequest request, CancellationToken cancellationToken)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }

            var command = _mapper.Map<UpdateScreenPostValidationStatusCommand, CloseScreenPostRequest>(request);
            command.Screenkey = screenkey;
            command.Status = ScreenValidationResultStatus.ManualyValid;
            command.DiscordGuildId = allianceid;

            await _sender.Send(command, cancellationToken);
        }
        [HttpPatch("{screenkey}")]
        public async Task Reject(ulong allianceid, string screenkey, [FromBody] CloseScreenPostRequest request, CancellationToken cancellationToken)
        {
            var alliance = await _allianceService.GetAlliance(allianceid.ToString());
            if (alliance == null)
            {
                throw new BadHttpRequestException($"Le bot discord n'a pas été invité au serveur {allianceid}");

            }
            var command = _mapper.Map<UpdateScreenPostValidationStatusCommand, CloseScreenPostRequest>(request);
            command.Screenkey = screenkey;
            command.Status = ScreenValidationResultStatus.ManualyInvalid;
            command.DiscordGuildId = allianceid;

            await _sender.Send(command, cancellationToken);
        }

    }
}
