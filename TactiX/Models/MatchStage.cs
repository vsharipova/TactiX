using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TactiX.Models
{
    [Table("match_stage")]
    public class MatchStage
    {
        [Key]
        [Column("match_stage_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchStageId { get; set; }

        [ForeignKey("Match")]
        [Column("match_id")]
        public int MatchId { get; set; }

        [Column("stage_name")]
        [Required]
        [MaxLength(255)]
        public string StageName { get; set; }

        [Column("stage_type")]
        [Required]
        public long StageType { get; set; }

        [Column("hit_factor")]
        [Required]
        public double HitFactor { get; set; }

        [Column("num_of_spots")]
        [Required]
        public long NumOfSpots { get; set; }

        [Column("num_of_poppers")]
        [Required]
        public long NumOfPoppers { get; set; }

        [Column("num_of_plates")]
        [Required]
        public long NumOfPlates { get; set; }

        [Column("alphas_count")]
        [Required]
        public long AlphasCount { get; set; }

        [Column("deltas_count")]
        [Required]
        public long DeltasCount { get; set; }

        [Column("charlies_count")]
        [Required]
        public long CharliesCount { get; set; }

        [Column("misses_count")]
        [Required]
        public long MissesCount { get; set; }

        [Column("stage_time")]
        [Required]
        public TimeSpan StageTime { get; set; }

        public Match Match { get; set; }
    }
}
