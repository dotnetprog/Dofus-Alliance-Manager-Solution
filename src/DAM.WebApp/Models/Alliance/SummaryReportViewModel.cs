namespace DAM.WebApp.Models.Alliance
{
    public class SummaryReportViewModel
    {
        public Guid? SelectedBaremeAttaqueId { get; set; }
        public Guid? SelectedBaremeDefenseId { get; set; }

        public IReadOnlyCollection<BaremeViewModel> Baremes { get; set; }




    }
}
