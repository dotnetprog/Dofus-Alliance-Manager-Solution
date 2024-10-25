using System.ComponentModel.DataAnnotations;

namespace DAM.Domain.Enums
{
    public enum BotScreenBehaviorType
    {
        [Display(Name = "Commandes")]
        Commands = 1,
        [Display(Name = "Auto:Mapping Canal Nombre Ennemie")]
        ChannelEnnemyCountMapping = 2
    }
}
