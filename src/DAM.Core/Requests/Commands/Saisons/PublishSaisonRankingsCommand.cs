using DAM.Core.Abstractions.Requests;
using MediatR;


namespace DAM.Core.Requests.Commands.Saisons
{
    public class PublishSaisonRankingsCommand : ICommand<Unit>
    {
        public Guid SaisonId { get; set; }
        public ulong GuidServerId { get; set; }

        public bool ClearChannel { get; set; }

    }
}
