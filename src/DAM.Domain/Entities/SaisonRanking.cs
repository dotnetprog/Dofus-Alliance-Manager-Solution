using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public class SaisonRanking
    {
        public Guid Id { get; set; }

        public Guid SaisonId { get; set; }
        [ForeignKey("SaisonId")]
        public Saison Saison { get; set; }

        public Guid MemberId { get; set; }
        [ForeignKey("MemberId")]
        public AllianceMember Member { get; set; }

        public int? Previous_PositionRank { get; set; }
        public int? Previous_Nombre_attaques { get; set; }
        public int? Previous_Nombre_defense { get; set; }
        public int? Previous_NombreParticipationAvA { get; set; }
        public int? Previous_MontantDefPepites { get; set; }
        public int? Previous_MontantAtkPepites { get; set; }
        public int? Previous_MontantAvAPepites { get; set; }
        public int? Previous_MontantTotalPepite { get; set; }
        public int? Previous_BonusPepite { get; set; }
        public string? Previous_bonusReason { get; set; }

        public int PositionRank { get; set; }
        public int Nombre_attaques { get; set; }
        public int Nombre_defense { get; set; }
        public int NombreParticipationAvA { get; set; }
        public int MontantDefPepites { get; set; }
        public int MontantAtkPepites { get; set; }

        public int MontantAvAPepites { get; set; }

        public int MontantTotalPepite { get; set; }
        public int? BonusPepite { get; set; }
        public string? bonusReason { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public AllianceMember? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid? ModifiedById { get; set; }
        [ForeignKey("ModifiedById")]
        public AllianceMember? ModifiedBy { get; set; }


    }
}
