using DAM.Core.Requests.Commands;
using FluentValidation;

namespace DAM.Core.Validations
{
    public class AddScreenPostCommandValidator : AbstractValidator<AddScreenPostCommand>
    {
        public AddScreenPostCommandValidator()
        {

            RuleFor(c => c.ImageUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _));
            RuleFor(c => c.ImagePrepUrl).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.ImagePrepUrl));
            RuleFor(c => c.DiscordGuildId).Custom((gid, ctx) =>
            {
                if (gid == 0)
                {
                    ctx.AddFailure("Should be greater than 0");
                }
            });
            RuleFor(c => c.Type).NotNull();
            RuleFor(c => c.Target).NotNull();
            RuleFor(c => c.DiscordChannelId).NotNull();
            RuleFor(c => c.DiscordMessageId).NotNull();
            RuleFor(c => c.OverridenCreatedByMemberId).NotNull();
            When(c => c.Type == Domain.Entities.ScreenPostType.Attack,
                () =>
                {
                    RuleFor(c => c.EnemyCount).InclusiveBetween(0, 5);
                }).Otherwise(() =>
                {
                    RuleFor(c => c.EnemyCount).InclusiveBetween(1, 5);
                });
            RuleFor(c => c.AllianceMembers).NotEmpty().ListCountMustBeInRange(1, 5);


        }

    }
}
