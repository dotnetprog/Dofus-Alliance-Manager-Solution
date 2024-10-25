using DAM.Core.Abstractions.Requests;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;
using MediatR;

namespace DAM.Core.Requests.Commands.Saisons
{
    public class UpdateSaisonRankingCommandHandler : ICommandHandler<UpdateSaisonRankingCommand, SaisonRanking>
    {
        private readonly ISender _sender;
        private readonly ISaisonServiceAsync _saisonServiceAsync;
        private readonly IAllianceManagementServiceAsync _allianceManagementServiceAsync;
        public UpdateSaisonRankingCommandHandler(ISaisonServiceAsync saisonServiceAsync, IAllianceManagementServiceAsync allianceManagementServiceAsync, ISender sender)
        {
            _saisonServiceAsync = saisonServiceAsync;
            _allianceManagementServiceAsync = allianceManagementServiceAsync;
            _sender = sender;
        }

        public async Task<SaisonRanking> Handle(UpdateSaisonRankingCommand request, CancellationToken cancellationToken)
        {

            var existingSaisonRanking = await _saisonServiceAsync.GetRanking(request.AllianceId, request.SaisonRankingId);

            if (existingSaisonRanking == null)
            {
                throw new RecordNotFoundException(request.SaisonRankingId, nameof(SaisonRanking));
            }
            var createdby = await _allianceManagementServiceAsync.GetMember(existingSaisonRanking.CreatedById.Value);
            var bonusToAdd = GetBonusToAdd(existingSaisonRanking.BonusPepite, request.Bonus);

            existingSaisonRanking.bonusReason = request.BonusReason;
            existingSaisonRanking.BonusPepite = request.Bonus;
            existingSaisonRanking.ModifiedById = request.ModifiedByUserId;
            existingSaisonRanking.ModifiedOn = DateTime.UtcNow;

            existingSaisonRanking.MontantTotalPepite += bonusToAdd;
            await _saisonServiceAsync.UpdateRanking(existingSaisonRanking);


            await _sender.Send(new GenerateSaisonRankingsCommand
            {
                AllianceId = request.AllianceId,
                CreatedByUserId = ulong.Parse(createdby.DiscordId),
                SaisonId = existingSaisonRanking.SaisonId
            });


            return existingSaisonRanking;

        }
        private int GetBonusToAdd(int? existingBonus, int? bonusToAdd)
        {
            var newbonus = bonusToAdd ?? 0;
            if (existingBonus.HasValue)
            {

                return ((newbonus - existingBonus.Value));
            }

            return newbonus;
        }
    }
}
