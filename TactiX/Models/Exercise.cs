using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TactiX.Models
{
    public enum ExerciseCategory
    {
        Accuracy,       
        Transfers,      
        DrawSpeed,     
        MovementSpeed,  
        ShootingOnMove, 
        Combination     
    }

    public enum DifficultyLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    [Table("exercise")]
    public class Exercise
    {
        [Key]
        [Column("exercise_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExerciseId { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("description")]
        [Required]
        public string Description { get; set; }

        [Column("category")]
        [Required]
        public ExerciseCategory Category { get; set; }

        [Column("image_url")]
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Column("tags", TypeName = "jsonb")]
        public string? TagsJson { get; set; }

        [Column("difficulty")]
        public DifficultyLevel Difficulty { get; set; }

        [Column("duration")]
        public TimeSpan? Duration { get; set; }

        [NotMapped]
        public List<string> Tags
        {
            get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>();
            set => TagsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}