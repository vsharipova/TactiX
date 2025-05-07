using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TactiX.Models
{
    [Table("training_analysis")]
    public class TrainingAnalysis
    {
        [Key]
        [Column("training_analysis_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingAnalysisId { get; set; }

        [Column("training_id")]
        [Required]
        [ForeignKey("Training")]
        public int TrainingId { get; set; }
        public Training Training { get; set; }

        [Column("training_type")]
        [Required]
        public string TrainingType { get; set; }

        [Column("total_shots")]
        [Required]
        public int TotalShots { get; set; }

        [Column("total_alphas")]
        [Required]
        public int TotalAlphas { get; set; }

        [Column("total_deltas")]
        [Required]
        public int TotalDeltas { get; set; }

        [Column("total_charlies")]
        [Required]
        public int TotalCharlies { get; set; }

        [Column("total_misses")]
        [Required]
        public int TotalMisses { get; set; }

        [Column("alpha_percentage")]
        [Required]
        public decimal AlphaPercentage { get; set; }

        [Column("delta_percentage")]
        [Required]
        public decimal DeltaPercentage { get; set; }

        [Column("charlie_percentage")]
        [Required]
        public decimal CharliePercentage { get; set; }

        [Column("miss_percentage")]
        [Required]
        public decimal MissPercentage { get; set; }

        [Column("avg_hit_factor")]
        [Required]
        public decimal AvgHitFactor { get; set; }

        [Column("performance_score")]
        [Required]
        [Range(0, 10)]
        public decimal PerformanceScore { get; set; }

        [Column("is_best_performance")]
        [Required]
        public bool IsBestPerformance { get; set; } = false;

        [Column("calculated_at")]
        [Required]
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Comparison> ComparisonsAsBase { get; set; }
        public ICollection<Comparison> ComparisonsAsTarget { get; set; }
    }
}