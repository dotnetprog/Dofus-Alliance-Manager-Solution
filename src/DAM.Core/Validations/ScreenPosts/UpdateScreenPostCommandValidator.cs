using DAM.Core.Requests.Commands.ScreenPosts;
using FluentValidation;

namespace DAM.Core.Validations
{
    public class UpdateScreenPostCommandValidator : AbstractValidator<UpdateScreenPostCommand>
    {
        public UpdateScreenPostCommandValidator()
        {
            RuleFor(c => c.ImageUrl).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.ImageUrl));
            RuleFor(c => c.ImagePrepUrl).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.ImagePrepUrl));
            RuleFor(c => c.DiscordGuildId).Custom((gid, ctx) =>
            {
                if (gid == 0)
                {
                    ctx.AddFailure("Should be greater than 0");
                }
            });

            RuleFor(c => c.DiscordMessageId).NotNull();
            RuleFor(c => c.AllianceMembers).ListCountMustBeInRange(1, 5).When(c => c.AllianceMembers != null && c.AllianceMembers.Any());

        }
    }
}
