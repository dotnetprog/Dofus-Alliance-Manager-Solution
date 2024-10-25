using DAM.Domain.Entities;

namespace DAM.WebApp.Models.Alliance
{
    public class BaremeViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }    

        public IEnumerable<BaremeDetailViewModel> baremeDetails { get; set; }


        public IEnumerable<Guid> Enemies { get; set; }

        public BaremeType? BaremeType { get; set; }
    }
}
