using System.ComponentModel.DataAnnotations;

namespace TactiX.Models
{
    public class MatchCreateDto
    {
        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Дата обязательна")]
        public DateTime Date { get; set; }
    }
}
