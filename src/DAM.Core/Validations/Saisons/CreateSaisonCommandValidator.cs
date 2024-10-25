using DAM.Core.Requests.Commands;
using FluentValidation;

namespace DAM.Core.Validations.Saisons
{
    public class CreateSaisonCommandValidator : AbstractValidator<CreateSaisonCommand>
    {
        public CreateSaisonCommandValidator()
        {
            RuleFor(c => c.CreatedByUserId).NotEmpty();
            RuleFor(c => c.AllianceId).NotEmpty();
            RuleFor(c => c.StartDate).NotEmpty();
            RuleFor(c => c.EndDate).NotEmpty();
            RuleFor(c => c.Name).NotNull().NotEmpty();
        }
    }
}
