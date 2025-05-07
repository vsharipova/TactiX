using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TactiX.Models
{
    [Table("match")]
    public class Match
    {
        [Key]
        [Column("match_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchId { get; set; }

        [Column("match_name")]
        [Required]
        [MaxLength(255)]
        public string MatchName { get; set; } 

        [Column("match_date", TypeName = "timestamp without time zone")]
        [Required]
        public DateTime MatchDate { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; } 


        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("match_analysis_id")]
        [ForeignKey("Analysis")]
        public int? MatchAnalysisId { get; set; }
        public MatchAnalysis Analysis { get; set; }

        public ICollection<MatchStage> Stages { get; set; } = new List<MatchStage>();
        public ICollection<Comparison> ComparisonsWhereBase { get; set; }
        public ICollection<Comparison> ComparisonsWhereTarget { get; set; }
    }
}
