using DAM.Domain.Entities;
using DAM.Domain.Ïdentity;
using Microsoft.EntityFrameworkCore;

namespace DAM.Database.Contexts
{
    public class AllianceContext : DbContext
    {
        public AllianceContext(DbContextOptions options) : base(options)
        {


        }
        public AllianceContext() { }
        public DbSet<AllianceMember_ScreenPost> Member_ScreenPosts { get; set; }
        public DbSet<AllianceConfiguration> AllianceConfigurations { get; set; }
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<AllianceMember> Members { get; set; }
        public DbSet<ScreenPost> ScreenPosts { get; set; }
        public DbSet<ScreenValidation> ScreenValidations { get; set; }
        public DbSet<Bareme> Baremes { get; set; }
        public DbSet<BaremeDetail> BaremeDetails { get; set; }

        public DbSet<DefScreensPost> DefScreensPosts { get; set; }
        public DbSet<AtkScreensPost> AtkScreensPosts { get; set; }

        public DbSet<SummaryReportRow> SummaryReportRows { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AllianceEnemy> Enemies { get; set; }
        public DbSet<AvA> AvA { get; set; }
        public DbSet<AvaMember> AvaMembers { get; set; }
        public DbSet<Saison> Saisons { get; set; }
        public DbSet<SaisonRanking> SaisonRankings { get; set; }
        public DbSet<AnkamaPseudo> AnkamaPseudos { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AllianceMember_ScreenPost>()
                .HasKey(t => new { t.ScreenPostId, t.AllianceMemberId });


            modelBuilder.Entity<Saison>().Property(c => c.State).HasConversion<int>();
            //AvaMember
            modelBuilder.Entity<AvaMember>().Property(c => c.ValidationState).HasConversion<int>();

            modelBuilder.Entity<AvA>().Property(c => c.Zone).HasConversion<int>();
            modelBuilder.Entity<AvA>().Property(c => c.ResultState).HasConversion<int?>();
            modelBuilder.Entity<AvA>().Property(c => c.State).HasConversion<int?>();
            modelBuilder.Entity<Bareme>().Property(c => c.Type).HasConversion<int?>();
            modelBuilder.Entity<AllianceConfiguration>().Property(c => c.BotScreenBehaviorType).HasConversion<int?>();
            modelBuilder.Entity<ScreenPost>().Property(c => c.Type).HasConversion<int>();
            modelBuilder.Entity<ScreenPost>().Property(c => c.Target).HasConversion<int>();
            modelBuilder.Entity<AllianceMember>().Property(c => c.State).HasConversion<int>();
            modelBuilder.Entity<AllianceMember>().Property(c => c.Status).HasConversion<int>();
            modelBuilder.Entity<AllianceMember>().HasMany(c => c.AnkamaPseudos).WithOne(c => c.AllianceMember);

            modelBuilder.Entity<ScreenValidation>().Property(c => c.ProcessingState).HasConversion<int>();
            modelBuilder.Entity<ScreenValidation>().Property(c => c.ResultState).HasConversion<int?>();

            modelBuilder.Entity<DefScreensPost>().Property(c => c.StatutTraitementValidation).HasConversion<int>();
            modelBuilder.Entity<DefScreensPost>().Property(c => c.StatutResultatValidation).HasConversion<int?>();

            modelBuilder.Entity<DefScreensPost>().ToView("DefScreens").HasKey(k => k.Id);
            modelBuilder.Entity<AtkScreensPost>().Property(c => c.StatutTraitementValidation).HasConversion<int>();
            modelBuilder.Entity<AtkScreensPost>().Property(c => c.StatutResultatValidation).HasConversion<int?>();
            modelBuilder.Entity<AtkScreensPost>().ToView("AtkScreens").HasKey(k => k.Id);

            modelBuilder.Entity<SummaryReportRow>().HasNoKey();
            modelBuilder.Entity<Alert>().HasKey(k => k.Id);
            modelBuilder.Entity<AllianceEnemy>().HasOne(a => a.BaremeDefense)
                .WithMany(a => a.EnemiesDefense)
                .HasForeignKey(a => a.BaremeDefenseId)
                .IsRequired(false);
            modelBuilder.Entity<AllianceEnemy>().HasOne(a => a.BaremeAttaque)
                .WithMany(a => a.EnemiesAttaque)
                .HasForeignKey(a => a.BaremeAttaqueId)
                .IsRequired(false);


        }

    }
}
