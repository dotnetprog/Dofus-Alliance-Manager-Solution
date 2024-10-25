using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    [Keyless]
    public class SummaryReportRow
    {
        public Guid MemberId { get; set; }
        public string DiscordId { get; set; }
        public string Username { get; set; }
        public int Nombre_attaques { get; set; }
        public int Nombre_defense { get; set; }
        public int NombreParticipationAvA { get; set; }
        public int MontantDefPepites { get; set; }
        public int MontantAtkPepites { get; set; }

        public int MontantAvAPepites { get; set; }

        [NotMapped]
        public int MontantTotalPepite => MontantAtkPepites + MontantDefPepites + MontantAvAPepites;


    }
}
