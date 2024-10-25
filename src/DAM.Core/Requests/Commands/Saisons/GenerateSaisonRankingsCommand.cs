using DAM.Core.Abstractions.Requests;
using MediatR;

namespace DAM.Core.Requests.Commands.Saisons
{
    public class GenerateSaisonRankingsCommand : ICommand<Unit>
    {
        public GenerateSaisonRankingsCommand() { }

        public Guid SaisonId { get; set; }
        public Guid AllianceId { get; set; }

        public ulong CreatedByUserId { get; set; }


    }
}
