using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TactiX.Models
{
    [Table("analysis")]
    public class Analysis
    {
        [Key]
        [Column("analysis_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalysisId { get; set; }

        [Column("period_start")]
        [Required]
        public DateOnly PeriodStart { get; set; }

        [Column("period_end")]
        [Required]
        public DateOnly PeriodEnd { get; set; }

        [Column("total_alphas")]
        [Required]
        public long TotalAlphas { get; set; }

        [Column("total_deltas")]
        [Required]
        public long TotalDeltas { get; set; }

        [Column("total_charlies")]
        [Required]
        public long TotalCharlies { get; set; }

        [Column("avg_hit_factor")]
        [Required]
        public double AvgHitFactor { get; set; }

        [Column("training_analysis_id")]
        [Required]
        public int TrainingAnalysisId { get; set; }

        [Column("match_analysis_id")]
        [Required]
        public int MatchAnalysisId { get; set; }

        [Column("overal_trend")]
        [Required]
        [MaxLength(255)]
        public string OverallTrend { get; set; }

        [Column("general_advice")]
        [Required]
        [MaxLength(255)]
        public string GeneralAdvice { get; set; }

        [ForeignKey("TrainingAnalysisId")]
        public TrainingAnalysis TrainingAnalysis { get; set; }

        [ForeignKey("MatchAnalysisId")]
        public MatchAnalysis MatchAnalysis { get; set; }
    }
}
