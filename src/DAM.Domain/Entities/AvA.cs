using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAM.Domain.Entities
{
    public enum ZoneAvA
    {
        [ChoiceDisplay("Frigost III - Bastion des froides légions")]
        FrigostMissiz = 1,
        [ChoiceDisplay("Frigost III - Caserne du jour sans fin")]
        FrigostIlyzaelle = 2,
        [ChoiceDisplay("Frigost III - Tannerie Écarlatte")]
        FrigostKlime = 3,
        [ChoiceDisplay("Frigost III - Remparts à vent")]
        FrigostSylargh = 4,
        [ChoiceDisplay("Frigost III - Jardins d'hiver")]
        FrigostNyleza = 5,
        [ChoiceDisplay("Frigost III - Tour de la clepsydre")]
        FrigostHarebourg = 6,
        [ChoiceDisplay("Frigost III - Royaume des Martegel")]
        FrigostDazak = 7,
        [ChoiceDisplay("Frigost I - Lac gelé")]
        FrigostMansot = 8,
        [ChoiceDisplay("Frigost I - Berceau d'alma")]
        FrigostBenLeRipate = 9,
        [ChoiceDisplay("Frigost I - Champs de glace")]
        FrigostRM = 10,
        [ChoiceDisplay("Frigost I - Larmes d'Ouronigride")]
        FrigostObsi = 11,
        [ChoiceDisplay("Frigost I - Crevasse Perge")]
        FrigostTengu = 12,
        [ChoiceDisplay("Frigost II - Forêt pétrifiée")]
        FrigostKorri = 13,
        [ChoiceDisplay("Frigost II - Crocs de verre")]
        FrigostKolosso = 14,
        [ChoiceDisplay("Frigost II - Ruche des gloursons")]
        FrigostGlour = 15,
        [ChoiceDisplay("Frigost II - Mont Torrideau")]
        FrigostTorrideau = 16,
        Otomai = 17,
        [ChoiceDisplay("Enutrosor - Creuset des fortunés")]
        EnutrosorCreuset = 18,
        [ChoiceDisplay("Enutrosor - Retraite des éternels")]
        EnutrosorRetraite = 19,
        [ChoiceDisplay("Enutrosor - Carrière Aurifère")]
        EnutrosorCarriere = 20,
        [ChoiceDisplay("Enutrosor - Fort thune")]
        EnutrosorFort = 21,
        [ChoiceDisplay("Srambad - Ruelle des Eaux-Suaires")]
        SrambadRuelle = 22,
        [ChoiceDisplay("Srambad - Hauts ténébreux")]
        SrambadTenebreux = 23,
        [ChoiceDisplay("Srambad - Catacombres")]
        SrambadCatacomb = 24,
        [ChoiceDisplay("Ecaflipus - Pierre de l'élévation")]
        EcaflipusPierre = 25,
        [ChoiceDisplay("Ecaflipus - Lande Poilue")]
        EcaflipusLande = 26,
        [ChoiceDisplay("Ecaflipus - Temple de Kerubim")]
        EcaflipusTemple = 27,
        [ChoiceDisplay("Xelorium - Chemins d'hier")]
        XeloriumHier = 28,
        [ChoiceDisplay("Xelorium - Jour Présent")]
        XeloriumPresent = 29,
        [ChoiceDisplay("Xelorium - Lendemains Incertains")]
        XeloriumDemain = 30,
        Amakna = 31,
        [ChoiceDisplay("île de Sakaï - Plaine de Sakaï")]
        SakaiPlaine = 32,
        [ChoiceDisplay("île de Sakaï - Forêt enneigée")]
        SakaiForet = 33,
        [ChoiceDisplay("Bonta - Coeur Immaculé")]
        BontaArene = 34,
        [ChoiceDisplay("Bonta - Autres")]
        Bonta = 35,
        [ChoiceDisplay("Brakmar - La Cuirasse")]
        BrakmarArene = 36,
        [ChoiceDisplay("Brakmar - Autres")]
        Brakmar = 37,
        [ChoiceDisplay("île des wabbits")]
        Wabbit = 38,
        [ChoiceDisplay("Laboratoire abandonné")]
        Labo = 39,
        [ChoiceDisplay("Village des brigandins")]
        Brigandin = 40,
        Inconnu = 999
    }
    public enum AvaResultState
    {
        Win = 1,
        Lost = 2
    }
    public enum AvaState
    {
        Open = 1,
        Closed = 2
    }
    public class AvA
    {
        public AvA()
        {
            this.Members = new HashSet<AvaMember>();
        }
        public Guid Id { get; set; }
        public string? Titre { get; set; }
        public AvaResultState? ResultState { get; set; }
        public ZoneAvA Zone { get; set; }

        public AvaState? State { get; set; }
        public string? ZoneAutres { get; set; }

        public int? MontantPepitesTotal { get; set; }
        public int? MontantPepitesObtenu { get; set; }
        [Precision(5, 2)]
        public decimal? PourcentagePaye { get; set; }
        public int? MontantPayeFixe { get; set; }
        public string? ImageUrl { get; set; }
        public Guid AllianceId { get; set; }
        [ForeignKey("AllianceId")]
        public Alliance Alliance { get; set; }

        public DateTime CreatedOn { get; set; }
        public Guid CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public AllianceMember CreatedBy { get; set; }
        public DateTime? ClosedOn { get; set; }
        public Guid? ClosedById { get; set; }
        [ForeignKey("ClosedById")]
        public AllianceMember? ClosedBy { get; set; }
        public ulong DiscordForumChannelId { get; set; }
        public ulong DiscordThreadChannelId { get; set; }


        public ICollection<AvaMember> Members { get; set; }


    }
}
