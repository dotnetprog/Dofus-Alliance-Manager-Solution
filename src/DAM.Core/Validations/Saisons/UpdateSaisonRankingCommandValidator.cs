using DAM.Core.Requests.Commands.Saisons;
using FluentValidation;

namespace DAM.Core.Validations.Saisons
{
    public class UpdateSaisonRankingCommandValidator : AbstractValidator<UpdateSaisonRankingCommand>
    {
        public UpdateSaisonRankingCommandValidator()
        {

            When((c) => c.Bonus.HasValue && c.Bonus.Value != 0, () =>
            {
                RuleFor(c => c.BonusReason).NotEmpty();
            });


        }
    }
}
