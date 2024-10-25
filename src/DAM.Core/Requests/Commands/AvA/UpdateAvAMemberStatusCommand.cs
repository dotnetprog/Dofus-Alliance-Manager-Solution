using DAM.Core.Abstractions.Requests;
using DAM.Domain.Entities;
using MediatR;

namespace DAM.Core.Requests.Commands.AvA
{
    public class UpdateAvAMemberStatusCommand : ICommand<Unit>
    {
        public Guid AvAMemberPresenceId { get; set; }
        public Guid AllianceId { get; set; }
        public AvAValidationState State { get; set; }
        public Guid ValidatedById { get; set; }
    }
}
