namespace DAM.WebApp.Models.Saison
{
    public class EditSaisonViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string SelectedChannelId { get; set; }
        public Guid? BaremeDefenseId { get; set; }
        public Guid? BaremeAttackId { get; set; }
    }
}
