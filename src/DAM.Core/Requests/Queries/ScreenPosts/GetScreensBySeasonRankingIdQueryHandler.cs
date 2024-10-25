using DAM.Core.Abstractions.Services;
using DAM.Core.Requests.Queries.Entities;
using DAM.Domain.Entities;
using MediatR;

namespace DAM.Core.Requests.Queries.ScreenPosts
{
    public class GetScreensBySeasonRankingIdQueryHandler : IRequestHandler<GetScreensBySeasonRankingIdQuery, IReadOnlyCollection<GetScreenPostQueryResult>>
    {
        private readonly IScreenPostServiceAsync screenPostServiceAsync;
        private readonly IBaremeServiceAsync baremeServiceAsync;
        private readonly IAllianceManagementServiceAsync allianceManagementServiceAsync;
        private readonly ISaisonServiceAsync saisonServiceAsync;



        public GetScreensBySeasonRankingIdQueryHandler(
            IScreenPostServiceAsync screenPostServiceAsync,
            IBaremeServiceAsync baremeServiceAsync,
            IAllianceManagementServiceAsync allianceManagementServiceAsync,
            ISaisonServiceAsync saisonServiceAsync)
        {
            this.screenPostServiceAsync = screenPostServiceAsync;
            this.baremeServiceAsync = baremeServiceAsync;
            this.allianceManagementServiceAsync = allianceManagementServiceAsync;
            this.saisonServiceAsync = saisonServiceAsync;
        }


        private int CalculateValue(Bareme bareme, ScreenPost screenpost, Guid MemberId)
        {
            var detail = bareme.Details.FirstOrDefault(d => d.NbAllie == (screenpost.Members.Sum(m => m.CharacterCount) ?? 0) && d.NbEnemie == screenpost.EnemyCount);

            var member_relation_screenpost = screenpost.Members.FirstOrDefault(m => m.AllianceMemberId == MemberId);

            return (detail.NbPepite * (member_relation_screenpost.CharacterCount ?? 0));

        }
        private GetScreenPostQueryResult BuildQueryResult(ScreenPost screenpost, IReadOnlyCollection<Bareme> baremes, Bareme atk, Bareme def, Guid MemberId)
        {
            var validation = screenpost.ScreenValidations.First();
            var isApproved = screenpost.ScreenValidations.Any(sv =>
                     sv.ResultState == ScreenValidationResultStatus.ManualyValid ||
                     sv.ResultState == ScreenValidationResultStatus.Valid);



            var selectedBareme = screenpost.Type == ScreenPostType.Attack ? atk : def;
            if (screenpost.AllianceEnemyId.HasValue)
            {
                var enemyBareme = screenpost.Type == ScreenPostType.Attack ?
                    baremes.FirstOrDefault(b => b.EnemiesAttaque.Any(e => e.Id == screenpost.AllianceEnemyId.Value)) :
                    baremes.FirstOrDefault(b => b.EnemiesDefense.Any(e => e.Id == screenpost.AllianceEnemyId.Value));
                if (enemyBareme != null)
                {
                    selectedBareme = enemyBareme;
                }
            }



            var value = isApproved ? CalculateValue(selectedBareme, screenpost, MemberId) : 0;
            return new GetScreenPostQueryResult
            {
                PointsValue = value,
                ImageUrl = screenpost.ImageUrl,
                CountAlly = (screenpost.Members.Sum(m => m.CharacterCount) ?? 0),
                CountEnemy = screenpost.EnemyCount,
                State = validation.ProcessingState,
                StateResult = validation.ResultState ?? ScreenValidationResultStatus.NotValid,
                Type = screenpost.Type,
                CreatedOn = screenpost.CreatedOn.ToUniversalTime(),
                AllianceEnemyName = screenpost.AllianceEnemy?.Name
            };
        }
        public async Task<IReadOnlyCollection<GetScreenPostQueryResult>> Handle(GetScreensBySeasonRankingIdQuery request, CancellationToken cancellationToken)
        {
            var alliance = await allianceManagementServiceAsync.GetAlliance(request.AllianceId);
            var screenRanking = await saisonServiceAsync.GetRanking(request.AllianceId, request.SeasonRankingId);
            var saison = await saisonServiceAsync.GetById(request.AllianceId, screenRanking.SaisonId);
            var baremeAtk = await baremeServiceAsync.GetBareme(alliance.Id, saison.BaremeAttackId ?? alliance.AllianceConfiguration.DefaultBaremeAttaqueId.Value);
            var baremeDefense = await baremeServiceAsync.GetBareme(alliance.Id, saison.BaremeDefenseId ?? alliance.AllianceConfiguration.DefaultBaremeDefenseId.Value);
            var existingBaremes = await baremeServiceAsync.GetBaremes(alliance.Id);




            var screens = await screenPostServiceAsync
                .GetScreenPostsByPlayerBetweenTwoDates(request.AllianceId, screenRanking.MemberId, saison.StartDate, saison.EndDate);





            return screens.Select(sp => BuildQueryResult(sp, existingBaremes, baremeAtk, baremeDefense, screenRanking.MemberId)).ToArray();


        }
    }
}
