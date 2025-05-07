using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TactiX.Models
{
    [Table("comparison")]
    public class Comparison
    {
        [Key]
        [Column("comparison_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComparisonId { get; set; }

        [Column("base_match_id")]
        public int BaseMatchId { get; set; }

        [Column("compared_match_id")]
        public int ComparedMatchId { get; set; }

        [ForeignKey("BaseMatchId")]
        public virtual MatchAnalysis BaseMatchAnalysis { get; set; }

        [ForeignKey("ComparedMatchId")]
        public virtual MatchAnalysis ComparedMatchAnalysis { get; set; }

        [Column("alpha_diff")]
        [Required]
        public decimal AlphaDiff { get; set; }

        [Column("delta_diff")]
        [Required]
        public decimal DeltaDiff { get; set; }

        [Column("charlie_diff")]
        [Required]
        public decimal CharlieDiff { get; set; }

        [Column("miss_diff")]
        [Required]
        public decimal MissDiff { get; set; }

        [Column("hit_factor_diff")]
        [Required]
        public decimal HitFactorDiff { get; set; }

        [Column("comparison_result")]
        [Required]
        [MaxLength(50)]
        public string ComparisonResult { get; set; } 

        [Column("advice")]
        [Required]
        public string Advice { get; set; }
    }
}