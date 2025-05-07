using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TactiX.Models
{
    [Table("training")]
    public class Training
    {
        [Key]
        [Column("training_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingId { get; set; }

        [Column("training_date", TypeName = "timestamp without time zone")]
        [Required]
        public DateTime TrainingDate { get; set; }

        [Column("type_of_training")]
        [Required]
        [MaxLength(255)]
        public string TypeOfTraining { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; } 


        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("training_analysis_id")]
        [ForeignKey("Analysis")]
        public int? TrainingAnalysisId { get; set; }
        public TrainingAnalysis Analysis { get; set; }

        public ICollection<TrainingStage> Stages { get; set; } = new List<TrainingStage>();
    }
}
