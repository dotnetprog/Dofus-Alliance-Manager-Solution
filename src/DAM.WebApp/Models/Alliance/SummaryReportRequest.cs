namespace DAM.WebApp.Models.Alliance
{
    public class SummaryReportRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public Guid? BaremeAttaqueId { get; set; }
        public Guid? BaremeDefenseId { get; set; }
        public decimal? Multiplier { get; set; }

    }
}
