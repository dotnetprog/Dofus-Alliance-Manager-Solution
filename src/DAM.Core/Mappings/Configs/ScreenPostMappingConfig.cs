using DAM.Core.Requests.Commands;
using DAM.Core.Requests.Commands.ScreenPosts;
using DAM.Domain.Entities;
using Mapster;

namespace DAM.Core.Mappings.Configs
{
    public class ScreenPostMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<AddScreenPostCommand, ScreenPost>()
                .Map(d => d.Description, s => s.Description)
                 .Map(d => d.ImageUrl, s => s.ImageUrl)
                 .Map(d => d.ImagePrepUrl, s => s.ImagePrepUrl)
                 .Map(d => d.AllianceEnemyId, s => s.AllianceEnemyId)
                 .Map(d => d.EnemyCount, s => s.EnemyCount)
                 .Map(d => d.Type, s => s.Type)
                 .Map(d => d.Target, s => s.Target)
                 .Map(d => d.DiscordChannelId, s => s.DiscordChannelId)
                 .Map(d => d.DiscordMessageId, s => s.DiscordMessageId)
                 .Map(d => d.CreatedOn, s => s.OverridenCreatedon);


            config.ForType<UpdateScreenPostCommand, ScreenPost>()
               .Map(d => d.Description, s => s.Description)
                .Map(d => d.ImageUrl, s => s.ImageUrl)
                .Map(d => d.ImagePrepUrl, s => s.ImagePrepUrl)
                .Map(d => d.AllianceEnemyId, s => s.AllianceEnemyId)
                .Map(d => d.DiscordMessageId, s => s.DiscordMessageId);

        }
    }
}
