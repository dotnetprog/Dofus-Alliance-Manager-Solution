using DAM.Domain.Entities;

namespace DAM.WebApp.Models.Alliance
{
    public class SummaryReport
    {
        public decimal TotalPepitesAtk { get; set; }
        public decimal TotalPepitesDef { get; set; }
        public decimal TotalPepitesAvA { get; set; }
        public decimal TotalPepites { get; set; }
        public decimal TotalPepitesObtenu { get; set; }
        public decimal MoyennePepiteParJoueur { get; set; }
        public IEnumerable<SummaryReportRowViewModel> data { get; set; }
    }
    public class SummaryReportRowViewModel
    {

        public SummaryReportRowViewModel(SummaryReportRow sr)
        {
            this.MemberId = sr.MemberId;
            this.DiscordId = sr.DiscordId;
            this.Username = sr.Username;

            this.Nombre_attaques = sr.Nombre_attaques;
            this.Nombre_defense = sr.Nombre_defense;
            this.NombreParticipationAvA = sr.NombreParticipationAvA;
            this.MontantDefPepites = sr.MontantDefPepites;
            this.MontantAtkPepites = sr.MontantAtkPepites;
            this.MontantAvAPepites = sr.MontantAvAPepites;
            this.MontantTotalPepites = this.MontantDefPepites + this.MontantAtkPepites + this.MontantAvAPepites;

        }


        public Guid MemberId { get; set; }
        public string DiscordId { get; set; }
        public string Username { get; set; }
        public int Nombre_attaques { get; set; }
        public int Nombre_defense { get; set; }
        public int NombreParticipationAvA { get; set; }
        public decimal MontantDefPepites { get; set; }
        public decimal MontantAtkPepites { get; set; }

        public decimal MontantAvAPepites { get; set; }
        public decimal MontantTotalPepites { get; set; }
    }
}
