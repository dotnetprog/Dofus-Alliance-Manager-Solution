using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using MediatR;

namespace DAM.Core.Requests.Commands.AvA
{
    public class RegisterAvaPlayerCommandHandler : IRequestHandler<RegisterAvaPlayerCommand, Guid>
    {

        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IAvAService _avaService;
        private readonly IDiscordBotService _discordBotService;

        public RegisterAvaPlayerCommandHandler(IAllianceManagementServiceAsync allianceManagementService,
            IAvAService avaService,
            IDiscordBotService discordBotService)
        {
            _allianceManagementService = allianceManagementService;
            _avaService = avaService;
            _discordBotService = discordBotService;
        }
        public async Task<Guid> Handle(RegisterAvaPlayerCommand request, CancellationToken cancellationToken)
        {

            var alliance = await _allianceManagementService.GetAlliance(request.AllianceId);

            var user = await _discordBotService.GetGuildUser(ulong.Parse(alliance.DiscordGuildId), request.PlayerDiscordId);

            var member = await _allianceManagementService.GetOrCreateAllianceMember(alliance.Id, user);

            var participation = new AvaMember()
            {
                AvaId = request.AvAId,
                ImageUrl = request.ScreenAttachmentUrl,
                DiscordMessageId = request.DiscordMessageId,
                MemberId = member.Id
            };

            var existing = await _avaService.GetMember(request.AvAId, member.Id);
            if (existing == null)
            {
                participation.Id = await this._avaService.RegisterAvAMember(participation);
            }
            else
            {
                participation.Id = existing.Id;
                await this._avaService.EditAvAMember(participation);
            }
            return participation.Id;
        }
    }
}
