using DAM.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Ïdentity
{
    public class AppUser : User
    {
        public Guid AllianceId { get; set; }
        [ForeignKey("AllianceId")]
        public Alliance Alliance { get; set; }
        public string? HashedClientSecret { get; set; }


    }
}
