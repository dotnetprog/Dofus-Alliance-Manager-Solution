using DAM.Bot.Commands.AutoComplete;
using DAM.Domain.Entities;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM.Bot.Commands.ComplexTypes
{

    public class ScreenCommandInput
    {
        [ComplexParameterCtor]
        public ScreenCommandInput(IAttachment file,
            [MaxValue(5),MinValue(0)] int NombreEnemies, ScreenPostTarget AttaqueType,
            IGuildUser Joueur1,
            IGuildUser? Joueur2 = null,
            IGuildUser? Joueur3 = null,
            IGuildUser? Joueur4 = null,
            IGuildUser? Joueur5 = null,
            [Autocomplete(typeof(AutoCompleteEnemiesModule))]string? AllianceEnnemie = null,
            string? Description = null) {
            this.file = file;
            this.Description = Description;
            this.AllianceEnnemi = AllianceEnnemie;
                this.NombreEnemies = NombreEnemies;
            this.Joueur1 = Joueur1;
            this.Joueur2 = Joueur2;
            this.Joueur3 = Joueur3;
            this.Joueur4 = Joueur4;
            this.Joueur5 = Joueur5;
            this.AttaqueType = AttaqueType;


        }
        public AllianceEnemy Enemy { get; set; }
        [RequiredInput(true)]
        public IAttachment file { get; set; }
        [RequiredInput(true)]
        public int NombreEnemies { get; set; }
        [RequiredInput(true)]
        public ScreenPostTarget AttaqueType { get; set; }
        [RequiredInput(true)]
        public IGuildUser Joueur1 { get; set; }
        [RequiredInput(false)]
        public IGuildUser? Joueur2 { get; set; }
        [RequiredInput(false)]
        public IGuildUser? Joueur3 { get; set; }
        [RequiredInput(false)]
        public IGuildUser? Joueur4 { get; set; }
        [RequiredInput(false)]
        public IGuildUser? Joueur5 { get; set; }
        [RequiredInput(false)]
        public string? AllianceEnnemi { get; set; }
        [RequiredInput(false)]
        public string? Description { get; set; }

        public IEnumerable<IGuildUser> GetAttaquants()
        {
            var attaquants = new List<IGuildUser?>() {
                Joueur1,
                Joueur2,
                Joueur3,
                Joueur4,
                Joueur5
            };
            return attaquants.Where(a => a != null).ToList();
        }
        public virtual ScreenPost ConvertToScreenPost()
        {
            var post = new ScreenPost()
            {
                ImageUrl = this.file.Url,
                Filesize = this.file.Size,
                EnemyCount = this.NombreEnemies,
                Description = this.Description,
                Target = this.AttaqueType,
                HasOtherWithSameSize = null
            };
            return post;
        }
    }


    public class AtkScreenCommandInput : ScreenCommandInput
    {
        public AtkScreenCommandInput(IAttachment file,
           [MaxValue(5), MinValue(0)] int NombreEnnemis, ScreenPostTarget AttaqueType,
           IGuildUser Joueur1,
           IGuildUser? Joueur2 = null,
           IGuildUser? Joueur3 = null,
           IGuildUser? Joueur4 = null,
           IGuildUser? Joueur5 = null,
           [Autocomplete(typeof(AutoCompleteEnemiesModule))] string? AllianceEnnemie = null,
           string? Description = null,
           IAttachment? ScreenPrepa = null) : base(file,
               NombreEnnemis, 
               AttaqueType, 
               Joueur1, Joueur2, 
               Joueur3, Joueur4, 
               Joueur5,AllianceEnnemie ,Description)
        {
            
            this.ScreenPrepa = ScreenPrepa;
        }
        public IAttachment? ScreenPrepa { get; set;}
        public override ScreenPost ConvertToScreenPost()
        {
            var post = base.ConvertToScreenPost();
            if(ScreenPrepa!= null)
            {
                post.ImagePrepUrl = this.ScreenPrepa.Url;
            }
            return post;
        }
    }
}
