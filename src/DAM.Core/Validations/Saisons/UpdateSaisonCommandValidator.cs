using DAM.Core.Requests.Commands;
using FluentValidation;

namespace DAM.Core.Validations.Saisons
{
    public class UpdateSaisonCommandValidator : AbstractValidator<UpdateSaisonCommand>
    {
        public UpdateSaisonCommandValidator()
        {

            RuleFor(c => c.ModifiedByDiscordId).NotEmpty();
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.AllianceId).NotEmpty();
            RuleFor(c => c.StartDate).NotEmpty();
            RuleFor(c => c.EndDate).NotEmpty();
            RuleFor(c => c.Name).NotNull().NotEmpty();

        }
    }
}
