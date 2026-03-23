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

        [Range(0, int.MaxValue)]
        public int NumOfSpots { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int NumOfPoppers { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int NumOfPlates { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int AlphasCount { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int DeltasCount { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int CharliesCount { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int MissesCount { get; set; } = 0;

        public string StageTimeInput { get; set; }

        [Required]
        public TimeSpan StageTime =>
        TimeSpan.FromSeconds(double.Parse(StageTimeInput.Replace(',', '.'), CultureInfo.InvariantCulture));
    }
}
