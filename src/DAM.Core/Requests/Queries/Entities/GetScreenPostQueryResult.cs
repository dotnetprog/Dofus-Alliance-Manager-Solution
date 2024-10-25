using DAM.Domain.Entities;

namespace DAM.Core.Requests.Queries.Entities
{
    public class GetScreenPostQueryResult
    {


        public string ImageUrl { get; set; }
        public int CountAlly { get; set; }
        public int CountEnemy { get; set; }

        public ScreenPostType Type { get; set; }
        public ScreenValidationResultStatus StateResult { get; set; }
        public ScreenValidationStatus State { get; set; }
        public int? PointsValue { get; set; }

        public string AllianceEnemyName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
