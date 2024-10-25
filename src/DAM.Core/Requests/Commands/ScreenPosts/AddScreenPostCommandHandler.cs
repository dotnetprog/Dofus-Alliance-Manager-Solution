using DAM.Core.Abstractions.Mapping;
using DAM.Core.Abstractions.Requests;
using DAM.Core.Abstractions.Services;
using DAM.Core.Events.Screens;
using DAM.Domain.Entities;
using DAM.Domain.Exceptions;
using MediatR;

namespace DAM.Core.Requests.Commands.ScreenPosts
{
    public class AddScreenPostCommandHandler : ICommandHandler<AddScreenPostCommand, Guid>
    {
        private readonly IAllianceManagementServiceAsync _allianceManagementService;
        private readonly IScreenPostServiceAsync _screenPostService;
        private readonly IDiscordBotService _botService;
        private readonly IDAMMapper _mapper;
        private readonly IPublisher _publisher;
        public AddScreenPostCommandHandler(IAllianceManagementServiceAsync allianceManagementServiceAsync,
                                            IScreenPostServiceAsync screenPostServiceAsync,
                                            IDiscordBotService botService,
                                            IDAMMapper mapper,
                                            IPublisher publisher)
        {
            _allianceManagementService = allianceManagementServiceAsync;
            _screenPostService = screenPostServiceAsync;
            _botService = botService;
            _mapper = mapper;
            _publisher = publisher;
        }





        public async Task<Guid> Handle(AddScreenPostCommand request, CancellationToken cancellationToken)
        {
            var alliance = await this._allianceManagementService.GetAlliance(request.DiscordGuildId.ToString());




            var screenMembers = await GetMembersFromDiscordIds(alliance.Id, request.DiscordGuildId, request.AllianceMembers.ToArray());
            var createdby = await GetOrCreateMember(alliance.Id, request.DiscordGuildId, request.OverridenCreatedByMemberId.Value);
            var screenpost = _mapper.Map<ScreenPost, AddScreenPostCommand>(request);
            screenpost.CreatedByMemberId = createdby.Id;

            var existingscreen = await _screenPostService.GetPost(request.DiscordMessageId.Value, false);
            if (existingscreen != null)
            {
                throw new InvalidEntityOperationException(existingscreen.Id,
                    nameof(ScreenPost),
                    EntityOperation.Create,
                    $"discord messageid {existingscreen.DiscordMessageId} is already associated with a screenpost.");
            }

            screenpost.Id = await this._screenPostService.CreatePost(screenpost, screenMembers.ToArray());

            await _publisher.Publish(new ScreenCreatedEvent { ScreenPost = screenpost });


            return screenpost.Id;
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
