using System.ComponentModel.DataAnnotations.Schema;


namespace DAM.Domain.Entities
{

    public enum SaisonState
    {
        Active = 0,
        Inactive = 1
    }

    public class Saison
    {
        public Saison()
        {

            this.Rankings = new HashSet<SaisonRanking>();
            this.State = SaisonState.Active;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ulong? SeasonRankingChannelId { get; set; }

        public Guid AllianceId { get; set; }
        [ForeignKey("AllianceId")]
        public Alliance Alliance { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? CreatedById { get; set; }

        [ForeignKey("CreatedById")]

        public AllianceMember? CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid? ModifiedById { get; set; }
        [ForeignKey("ModifiedById")]
        public AllianceMember? ModifiedBy { get; set; }

        public SaisonState State { get; set; }
        public DateTime? LadderGeneratedOn { get; set; }


        public Guid? BaremeDefenseId { get; set; }

        public Guid? BaremeAttackId { get; set; }
        [ForeignKey("BaremeDefenseId")]
        public Bareme? BaremeDefense { get; set; }
        [ForeignKey("BaremeAttackId")]
        public Bareme? BaremeAttack { get; set; }
        public ICollection<SaisonRanking> Rankings { get; set; }

    }
}
