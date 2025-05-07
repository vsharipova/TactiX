using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TactiX.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("surname")]
        [Required]
        [MaxLength(255)]
        public string Surname { get; set; }

        [Column("email")]
        [Required]
        public string Email { get; set; }

        [Column("hash_password")]
        [Required]
        public string HashPassword { get; set; }

        [Column("practice_level")]
        [Required]
        [MaxLength(255)]
        public string PracticeLevel { get; set; }

        public ICollection<Training> Trainings { get; set; }
        public ICollection<Match> Matches { get; set; }
        public ICollection<Analysis> Analyses { get; set; }
        public ICollection<Comparison> Comparisons { get; set; }
    }
}
