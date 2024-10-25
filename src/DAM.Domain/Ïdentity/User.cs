using System.ComponentModel.DataAnnotations;

namespace DAM.Domain.Ïdentity
{
    public abstract class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
    }
}
