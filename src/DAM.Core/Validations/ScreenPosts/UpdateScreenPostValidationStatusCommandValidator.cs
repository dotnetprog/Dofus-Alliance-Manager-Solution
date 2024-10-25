using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Domain.Entities;
using FluentValidation;

namespace DAM.Core.Validations
{
    public class UpdateScreenPostValidationStatusCommandValidator : AbstractValidator<UpdateScreenPostValidationStatusCommand>
    {

        public UpdateScreenPostValidationStatusCommandValidator()
        {

            RuleFor(c => c.Screenkey).Custom((Screenkey, context) =>
            {
                var isValid = ulong.TryParse(Screenkey, out var messageid) || Guid.TryParse(Screenkey, out Guid PostId);
                if (isValid)
                    return;

                context.AddFailure("The Screenkey should be able to be parsed as a GUID or a ulong(bigint)");

            });
            RuleFor(c => c.Status).NotNull().Custom((status, context) =>
            {
                if (status != ScreenValidationResultStatus.ManualyValid && status != ScreenValidationResultStatus.ManualyInvalid)
                {
                    context.AddFailure($"Only the following values are supported for now: {ScreenValidationResultStatus.ManualyValid.ToString()}({(int)ScreenValidationResultStatus.ManualyValid}),{ScreenValidationResultStatus.ManualyInvalid.ToString()}({(int)ScreenValidationResultStatus.ManualyInvalid})");
                }


            });
            RuleFor(c => c.ClosedByDiscordUserId).Custom((ClosedByDiscordUserId, context) =>
            {
                var isValid = ulong.TryParse(ClosedByDiscordUserId, out var messageid);
                if (isValid)
                    return;

                context.AddFailure("The ClosedByDiscordUserId should be able to be parsed as a ulong(bigint)");

            });



        }

    }
}
