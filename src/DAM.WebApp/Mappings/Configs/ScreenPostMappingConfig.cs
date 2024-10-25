using DAM.Core.Requests.Commands;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.WebApp.Models.Api.ScreenPosts;
using Mapster;

namespace DAM.WebApp.Mappings.Configs
{
    public class ScreenPostMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<CreateScreenPostRequest, AddScreenPostCommand>()
                .Map(d => d.Description, s => s.Description)
                 .Map(d => d.ImageUrl, s => s.ImageUrl)
                 .Map(d => d.ImagePrepUrl, s => s.ImagePrepUrl)
                 .Map(d => d.EnemyCount, s => s.EnemyCount)
                 .Map(d => d.Type, s => s.Type)
                 .Map(d => d.Target, s => s.Target)
                 .Map(d => d.DiscordChannelId, s => ulong.Parse(s.DiscordChannelId))
                 .Map(d => d.DiscordMessageId, s => ulong.Parse(s.DiscordMessageId))
                 .Map(d => d.AllianceEnemyId, s => s.AllianceEnemyId)
                 .Map(d => d.OverridenCreatedon, s => s.OverridenCreatedon)
                 .Map(d => d.OverridenCreatedByMemberId, s => s.CreatedByDiscordUserId)
                 .Map(d => d.AllianceMembers, s => s.DiscordUserIds.Select(id => ulong.Parse(id)).ToArray());
            config.ForType<UpdateScreenPostRequest, UpdateScreenPostCommand>()
                .Map(d => d.Description, s => s.Description)
                 .Map(d => d.ImageUrl, s => s.ImageUrl)
                 .Map(d => d.ImagePrepUrl, s => s.ImagePrepUrl)
                 .Map(d => d.AllianceEnemyId, s => s.AllianceEnemyId)
                 .Map(d => d.AllianceMembers, s => s.DiscordUserIds.Select(id => ulong.Parse(id)).ToArray());
            config.ForType<CloseScreenPostRequest, UpdateScreenPostValidationStatusCommand>()
               .Map(d => d.ClosedByDiscordUserId, s => s.ClosedByDiscordUserId);


        }
    }
}
