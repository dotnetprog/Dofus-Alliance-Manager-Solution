using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Domain.Entities
{
    public class BaremeDetail
    {
        public Guid Id { get; set; }
        public Guid BaremeId { get; set; }
        [ForeignKey("BaremeId")]
        public Bareme Bareme { get; set; }


        public int NbEnemie { get; set; }
        public int NbAllie { get; set; }
        public int NbPepite { get; set; }

    }
}
