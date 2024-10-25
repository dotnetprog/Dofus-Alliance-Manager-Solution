using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Domain.Entities
{
    public class Alert
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public Guid CreatedByMemberId { get; set; }

        public DateTime CreatedOn { get; set;}
        [ForeignKey("CreatedByMemberId")]

        public AllianceMember CreatedBy { get; set; }

        public int? AudienceCount { get; set; }



    }
}
