using DAM.Domain.Entities;

namespace DAM.WebApp.Models.Alliance
{
    public class AddBaremeViewModel
    {
        public string Name { get; set; }

        public IEnumerable<CreateBaremeDetailViewModel> Details { get; set; }

        public IEnumerable<Guid> Enemies { get; set; }

        public BaremeType? BaremeType { get; set; }
        

    }
}
