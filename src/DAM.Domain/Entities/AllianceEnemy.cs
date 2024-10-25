using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public class AllianceEnemy
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AllianceId { get; set; }
        [ForeignKey("AllianceId")]
        public Alliance Alliance { get; set; }



        public Guid? BaremeDefenseId { get; set; }
        //  [ForeignKey("BaremeDefenseId")]
        public Bareme? BaremeDefense { get; set; }
        public Guid? BaremeAttaqueId { get; set; }
        // [ForeignKey("BaremeAttaqueId")]
        public Bareme? BaremeAttaque { get; set; }

    }
}
