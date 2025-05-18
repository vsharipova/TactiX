using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TactiX.Models
{
    [Table("training_stage")]
    public class TrainingStage
    {
        [Key]
        [Column("training_stage_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingStageId { get; set; }

        [ForeignKey("Training")]
        [Column("training_id")]
        public int TrainingId { get; set; }

        [Column("stage_name")]
        [Required]
        [MaxLength(255)]
        public string StageName { get; set; }

        [Column("stage_type")]
        [Required]
        [MaxLength(255)]
        public string StageType { get; set; }

        [Column("hit_factor")]
        [Required]
        public double HitFactor { get; set; }

        [Column("num_of_spots")]
        [Required]
        public int NumOfSpots { get; set; }

        [Column("num_of_poppers")]
        [Required]
        public int NumOfPoppers { get; set; }

        [Column("num_of_plates")]
        [Required]
        public int NumOfPlates { get; set; }

        [Column("alphas_count")]
        [Required]
        public int AlphasCount { get; set; }

        [Column("deltas_count")]
        [Required]
        public int DeltasCount { get; set; }

        [Column("charlies_count")]
        [Required]
        public int CharliesCount { get; set; }

        [Column("misses_count")]
        [Required]
        public int MissesCount { get; set; }

        [Column("stage_time")]
        [Required]
        public TimeSpan StageTime { get; set; }

        public Training Training { get; set; }
    }
}
