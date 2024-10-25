using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Domain.Entities
{
    public class BlackListMember
    {

        public Guid Id { get; set; }

        public Guid AllianceId { get; set; }
        [ForeignKey("AllianceId")]
        public Alliance Alliance { get; set; }

        public string? AnkadexUrl { get; set; }
        public string? AnkamaPseudo { get; set; }
        public string? PseudoData { get; set; }


        public string? reason { get; set; }

        public Guid? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public AllianceMember? CreatedBy { get; set; }


    }
}
