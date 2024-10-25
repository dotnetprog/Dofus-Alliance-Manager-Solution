
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;

namespace DAM.Core.Requests.Commands.AvA
{
    public class UpdateAvAMemberStatusCommandHandler : BaseSimpleCommandHandler<UpdateAvAMemberStatusCommand>
    {
        private readonly IAvAService _avaService;


        public UpdateAvAMemberStatusCommandHandler(
            IAvAService avaService)
        {
            _avaService = avaService;
        }
        public override async Task Run(UpdateAvAMemberStatusCommand request, CancellationToken cancellationToken)
        {

            var participation = new AvaMember
            {
                Id = request.AvAMemberPresenceId,
                ValidatedById = request.ValidatedById,
                ValidationState = request.State,
                DiscordMessageId = default(ulong)
            };
            await _avaService.EditAvAMember(participation);


        }
    }
}
