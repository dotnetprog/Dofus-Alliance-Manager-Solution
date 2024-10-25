using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Requests;
using DAM.Core.Abstractions.Services;
using DAM.Core.Abstractions.Validator;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace DAM.Core.Requests.Commands
{
    public class UpdateSaisonCommandHandler : ICommandHandler<UpdateSaisonCommand, Unit>
    {
        private readonly ISaisonServiceAsync _saisonServiceAsync;
        private readonly IDAMMapper _mapper;
        private readonly ISaisonValidator _saisonValidator;
        private readonly IDiscordBotService _botService;
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        public UpdateSaisonCommandHandler(ISaisonServiceAsync saisonServiceAsync,
            IDAMMapper mapper,
            ISaisonValidator saisonValidator,
            IDiscordBotService botService,
            IAllianceManagementServiceAsync allianceManagementService)
        {
            _saisonServiceAsync = saisonServiceAsync;
            _mapper = mapper;
            _saisonValidator = saisonValidator;
            _botService = botService;
            _allianceManagementService = allianceManagementService;
        }
        public async Task<Unit> Handle(UpdateSaisonCommand request, CancellationToken cancellationToken)
        {

            var alliance = await _allianceManagementService.GetAlliance(request.AllianceId);

            var modifiedby = await GetOrCreateMember(request.AllianceId, ulong.Parse(alliance.DiscordGuildId), request.ModifiedByDiscordId);
            var faillures = await _saisonValidator.ValidateCommand(request);
            if (faillures.Any())
            {
                throw new ValidationException(faillures);
            }

            var saison = _mapper.Map<Saison, UpdateSaisonCommand>(request);
            saison.ModifiedById = modifiedby.Id;
            await _saisonServiceAsync.Update(saison);
            return Unit.Value;

        }
        private async Task<AllianceMember> GetOrCreateMember(Guid AllianceId, ulong GuildId, ulong discordid)
        {
            var discorduser = await _botService.GetGuildUser(GuildId, discordid);
            if (discorduser == null)
            {
                throw new InvalidDiscordUserException(discordid, GuildId);
            }
            var member = await this._allianceManagementService.GetOrCreateAllianceMember(AllianceId, discorduser);
            return member;
        }
    }
}
