using Microsoft.EntityFrameworkCore;
using TactiX.Models;

namespace TactiX.DBContext
{
    public class TactiXDB : DbContext
    {
        public TactiXDB(DbContextOptions<TactiXDB> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingStage> TrainingStages { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchStage> MatchStages { get; set; }
        public DbSet<Analysis> Analyses { get; set; }
        public DbSet<TrainingAnalysis> TrainingAnalyses { get; set; }
        public DbSet<MatchAnalysis> MatchAnalyses { get; set; }
        public DbSet<Comparison> Comparisons { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => new { u.Name, u.Surname }).IsUnique();
            });

            modelBuilder.Entity<Training>()
                 .HasMany(t => t.Stages)
                 .WithOne(s => s.Training)
                 .HasForeignKey(s => s.TrainingId);

            modelBuilder.Entity<TrainingStage>(entity =>
            {
                entity.HasKey(ts => ts.TrainingStageId);
                entity.HasIndex(ts => new { ts.StageName, ts.HitFactor });
                entity.Property(ts => ts.BriefingData).HasColumnType("jsonb");
            });


            modelBuilder.Entity<Match>(entity =>
            {
               entity.HasMany(m => m.Stages)
               .WithOne(s => s.Match)
               .HasForeignKey(s => s.MatchId)
               .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Analysis)
                     .WithOne(ma => ma.Match)
                     .HasForeignKey<MatchAnalysis>(ma => ma.MatchId)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MatchStage>(entity =>
            {
                entity.HasKey(ms => ms.MatchStageId);
                entity.HasIndex(ms => new { ms.StageName, ms.HitFactor });
                entity.Property(ms => ms.BriefingData).HasColumnType("jsonb");
            });

            modelBuilder.Entity<Analysis>(entity =>
            {
                entity.HasKey(a => a.AnalysisId);
                entity.HasIndex(a => new { a.PeriodStart, a.PeriodEnd });

                entity.HasOne(a => a.TrainingAnalysis)
                     .WithMany()
                     .HasForeignKey(a => a.TrainingAnalysisId)
                     .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.MatchAnalysis)
                     .WithMany()
                     .HasForeignKey(a => a.MatchAnalysisId)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TrainingAnalysis>(entity =>
            {
                entity.HasKey(ta => ta.TrainingAnalysisId);

                entity.HasOne(ta => ta.Training)
                     .WithOne(t => t.Analysis) 
                     .HasForeignKey<TrainingAnalysis>(ta => ta.TrainingId)
                     .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ta => ta.IsBestPerformance);
                entity.HasIndex(ta => ta.PerformanceScore);
                entity.HasIndex(ta => ta.CalculatedAt);
                entity.HasIndex(ta => ta.TrainingType); 

                entity.Property(ta => ta.AlphaPercentage).HasColumnType("decimal(5,2)");
                entity.Property(ta => ta.DeltaPercentage).HasColumnType("decimal(5,2)");
                entity.Property(ta => ta.CharliePercentage).HasColumnType("decimal(5,2)");
                entity.Property(ta => ta.MissPercentage).HasColumnType("decimal(5,2)");
                entity.Property(ta => ta.AvgHitFactor).HasColumnType("decimal(5,2)");
                entity.Property(ta => ta.PerformanceScore).HasColumnType("decimal(3,1)");

                entity.Property(ta => ta.TrainingType)
                      .HasConversion<string>() 
                      .HasMaxLength(50); 
            });

            modelBuilder.Entity<MatchAnalysis>(entity =>
            {
                entity.HasKey(ma => ma.MatchAnalysisId);

                entity.HasOne(ma => ma.Match)
                     .WithOne(m => m.Analysis)
                     .HasForeignKey<MatchAnalysis>(ma => ma.MatchId)
                     .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ma => ma.IsBestPerformance);
                entity.HasIndex(ma => ma.PerformanceScore);
                entity.HasIndex(ma => ma.CalculatedAt);

                entity.Property(ma => ma.AlphaPercentage).HasColumnType("decimal(5,2)");
                entity.Property(ma => ma.DeltaPercentage).HasColumnType("decimal(5,2)");
                entity.Property(ma => ma.CharliePercentage).HasColumnType("decimal(5,2)");
                entity.Property(ma => ma.MissPercentage).HasColumnType("decimal(5,2)");
                entity.Property(ma => ma.AvgHitFactor).HasColumnType("decimal(5,2)");
                entity.Property(ma => ma.PerformanceScore).HasColumnType("decimal(3,1)");
            });

            modelBuilder.Entity<Comparison>(entity =>
            {
                entity.HasKey(c => c.ComparisonId);

                entity.HasOne(c => c.BaseMatchAnalysis)
                    .WithMany()
                    .HasForeignKey(c => c.BaseMatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ComparedMatchAnalysis)
                    .WithMany()
                    .HasForeignKey(c => c.ComparedMatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.ExerciseId);
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Difficulty);
                entity.Property(e => e.TagsJson).HasColumnType("jsonb");
                entity.Property(e => e.Category).HasConversion<string>();
                entity.Property(e => e.Difficulty).HasConversion<string>();
            });

            modelBuilder.HasDefaultSchema("public");
        }
    }
}
