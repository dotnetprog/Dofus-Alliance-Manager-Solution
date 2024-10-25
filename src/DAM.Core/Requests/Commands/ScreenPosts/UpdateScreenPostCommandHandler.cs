using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Services;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;

namespace DAM.Core.Requests.Commands.ScreenPosts
{
    public class UpdateScreenPostCommandHandler : BaseSimpleCommandHandler<UpdateScreenPostCommand>
    {

        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IScreenPostServiceAsync _screenPostService;
        private readonly IDiscordBotService _botService;
        private readonly IDAMMapper _mapper;
        public UpdateScreenPostCommandHandler(IAllianceManagementServiceAsync allianceManagementServiceAsync,
                                            IScreenPostServiceAsync screenPostServiceAsync,
                                            IDiscordBotService botService,
                                            IDAMMapper mapper)
        {
            _allianceManagementService = allianceManagementServiceAsync;
            _screenPostService = screenPostServiceAsync;
            _botService = botService;
            _mapper = mapper;
        }


        public override async Task Run(UpdateScreenPostCommand request, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<AllianceMember> members = null;
            var alliance = await this._allianceManagementService.GetAlliance(request.DiscordGuildId.ToString());

            var screenpost = this._mapper.Map<ScreenPost, UpdateScreenPostCommand>(request);
            if ((request.AllianceMembers?.Any() ?? false))
            {
                members = await GetMembersFromDiscordIds(alliance.Id, request.DiscordGuildId, request.AllianceMembers.ToArray());
            }
            await this._screenPostService.UpdatePost(screenpost, members?.ToArray());

        }
        private async Task<AllianceMember> GetOrCreateMember(Guid AllianceId, ulong GuildId, ulong discordid)
        {
            var discorduser = await _botService.GetGuildUser(GuildId, discordid);
            if (discorduser == null)
            {
                throw new InvalidDiscordUserException(discordid, GuildId);
            }
            var member = await this._allianceManagementService.GetOrCreateAllianceMember(AllianceId, discorduser);
            return member;
        }
        private async Task<IReadOnlyCollection<AllianceMember>> GetMembersFromDiscordIds(Guid AllianceId, ulong GuildId, ulong[] discordids)
        {
            var members = new List<AllianceMember>();
            foreach (var discordid in discordids)
            {
                var member = await GetOrCreateMember(AllianceId, GuildId, discordid);
                members.Add(member);

            }
            return members;
        }
    }
}
