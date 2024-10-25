using DAM.Domain.Entities;

namespace DAM.WebApp.Models.AvA
{
    public class AvAViewModel
    {
        public Guid Id { get; set; }
        public string title { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ClosedOn { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public AvaState State { get; set; }
        public AvaResultState? Statut { get; set; }

        public int? MontantTotal { get; set; }
        public int? MontantObtenu { get; set; }

        public int? MontantParJoueur { get; set; }

        public int NbParticipants { get; set; }

        public Guid? ClosedById { get; set; }
        public string? ClosedByName { get; set; }
        public decimal? Pourcentage { get; set; }

    }
}
