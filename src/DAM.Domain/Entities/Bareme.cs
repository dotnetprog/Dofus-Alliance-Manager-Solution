using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Domain.Entities
{

    public enum BaremeType
    {
        Attaque=1,
        Defense=2
    }
    public class Bareme
    {
        public Bareme() {
            this.EnemiesDefense = new HashSet<AllianceEnemy>();
            this.EnemiesAttaque = new HashSet<AllianceEnemy>();
        }
        public Guid Id { get;set;}
        public string Name { get;set;}
        public BaremeType? Type { get; set; }
        public Guid AllianceId { get;set;}
        [ForeignKey("AllianceId")]
        public Alliance Alliance { get;set;}
        public ICollection<BaremeDetail> Details { get; set;}
        public ICollection<AllianceEnemy> EnemiesDefense { get; set; }
        public ICollection<AllianceEnemy> EnemiesAttaque { get; set; }

        
    }
}
