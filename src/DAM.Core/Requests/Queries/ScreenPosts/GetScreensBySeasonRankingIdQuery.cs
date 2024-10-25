using DAM.Core.Requests.Queries.Entities;
using MediatR;

namespace DAM.Core.Requests.Queries.ScreenPosts
{
    public class GetScreensBySeasonRankingIdQuery : IRequest<IReadOnlyCollection<GetScreenPostQueryResult>>
    {
        public Guid AllianceId { get; set; }

        public Guid SeasonRankingId { get; set; }
    }
}
