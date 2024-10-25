using DAM.Domain.JsonData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Domain.Entities
{
    public class AnkamaPseudo
    {

        public Guid Id { get; set; }

        public string? Pseudo { get; set; }

        public string? AnkadexUrl { get; set; }
        public string? PseudoData { get; set; }

        public Guid AllianceMemberId { get; set; }
        [ForeignKey("AllianceMemberId")]
        public AllianceMember? AllianceMember { get; set; }
        
        public Guid? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public AllianceMember? CreatedBy { get; set; }

        public DateTime LastRefreshedOn { get; set; }


        public DateTime? AccountCreatedOn { get; set; }

        public AnkamaPseudoData[] GetData()
        {
            return JsonConvert.DeserializeObject<AnkamaPseudoData[]>(this.PseudoData);
        }

    }
}
