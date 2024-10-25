
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;

namespace DAM.Core.Requests.Commands.ScreenPosts
{
    public class UpdateScreenPostValidationStatusCommandHandler : BaseSimpleCommandHandler<UpdateScreenPostValidationStatusCommand>
    {
        private readonly IScreenPostServiceAsync _screenPostServiceAsync;
        private readonly IAllianceManagementServiceAsync _allianceService;
        private readonly IDiscordBotService _botService;
        public UpdateScreenPostValidationStatusCommandHandler(IAllianceManagementServiceAsync allianceService,
            IDiscordBotService botService, IScreenPostServiceAsync screenPostService)
        {
            _allianceService = allianceService;
            _botService = botService;
            _screenPostServiceAsync = screenPostService;
        }

        public async override Task Run(UpdateScreenPostValidationStatusCommand request, CancellationToken cancellationToken)
        {

            var alliance = await _allianceService.GetAlliance(request.DiscordGuildId.ToString());



            var closedbydiscorduser = await _botService.GetGuildUser(request.DiscordGuildId, ulong.Parse(request.ClosedByDiscordUserId));

            var closedby = await _allianceService.GetOrCreateAllianceMember(alliance.Id, closedbydiscorduser);


            ScreenPost post = null;

            if (Guid.TryParse(request.Screenkey, out Guid ScreenPostId))
            {
                post = await _screenPostServiceAsync.GetPost(ScreenPostId, false);
            }
            else if (ulong.TryParse(request.Screenkey, out ulong PostMessageId))
            {
                post = await _screenPostServiceAsync.GetPost(PostMessageId, false);
            }

            if (post == null)
            {
                throw new InvalidEntityOperationException(request.Screenkey, nameof(ScreenPost), EntityOperation.Update, "the record could not be found.");
            }





            await _screenPostServiceAsync.SetScreenValidationResult(post.Id, request.Status, closedby.Id);



        }
    }
}
