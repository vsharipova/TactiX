using System.ComponentModel.DataAnnotations;

namespace TactiX.Models.ViewModels
{
    public class TrainingCreateDto
    {
        [Required]
        public DateTime TrainingDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string TypeOfTraining { get; set; }
    }
}
