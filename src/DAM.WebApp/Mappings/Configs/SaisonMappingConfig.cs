using DAM.Core.Requests.Commands;
using DAM.WebApp.Models.Saison;
using Mapster;

namespace DAM.WebApp.Mappings.Configs
{
    public class SaisonMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<CreateSaisonViewModel, CreateSaisonCommand>()
              .Map(d => d.Description, s => s.Description)
               .Map(d => d.EndDate, s => s.EndDate)
               .Map(d => d.StartDate, s => s.StartDate)
               .Map(d => d.Name, s => s.Name)
               .Map(d => d.BaremeAttackId, s => s.BaremeAttackId)
               .Map(d => d.BaremeDefenseId, s => s.BaremeDefenseId)
               .Map(d => d.DiscordChannelId, s => ulong.Parse(s.SelectedChannelId));

        }
    }
}
