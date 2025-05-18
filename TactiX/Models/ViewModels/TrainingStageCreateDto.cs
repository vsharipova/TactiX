using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TactiX.Models.ViewModels
{
    public class TrainingStageCreateDto
    {
        [Required]
        public int TrainingId { get; set; }

        [Required]
        [MaxLength(255)]
        public string StageName { get; set; }

        [Required]
        [MaxLength(255)]
        public string StageType { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0000}")]
        public double HitFactor { get; set; }

        [Required]
        public int NumOfSpots { get; set; }

        [Required]
        public int NumOfPoppers { get; set; }

        [Required]
        public int NumOfPlates { get; set; }

        [Required]
        public int AlphasCount { get; set; }

        [Required]
        public int DeltasCount { get; set; }

        [Required]
        public int CharliesCount { get; set; }

        [Required]
        public int MissesCount { get; set; }

        public string StageTimeInput { get; set; }

        [Required]
        public TimeSpan StageTime =>
        TimeSpan.FromSeconds(double.Parse(StageTimeInput.Replace(',', '.'), CultureInfo.InvariantCulture));
    }
}
