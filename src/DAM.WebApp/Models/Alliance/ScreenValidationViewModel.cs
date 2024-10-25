using DAM.Domain.Entities;
using Discord;

namespace DAM.WebApp.Models.Alliance
{
    public class ScreenValidationViewModel
    {

        public Guid ScreenId { get; set; }

        public ScreenValidationResultStatus Result { get; set; } 
    }
}
