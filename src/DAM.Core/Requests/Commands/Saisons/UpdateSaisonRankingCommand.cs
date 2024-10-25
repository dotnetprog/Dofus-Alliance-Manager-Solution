using DAM.Core.Abstractions.Requests;
using DAM.Domain.Entities;

namespace DAM.Core.Requests.Commands.Saisons
{
    public class UpdateSaisonRankingCommand : ICommand<SaisonRanking>
    {
        public Guid SaisonRankingId { get; set; }
        public Guid ModifiedByUserId { get; set; }

        public Guid AllianceId { get; set; }
        public string BonusReason { get; set; }
        public int? Bonus { get; set; }

    }
}
