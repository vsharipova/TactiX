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
        public long NumOfSpots { get; set; }

        [Required]
        public long NumOfPoppers { get; set; }

        [Required]
        public long NumOfPlates { get; set; }

        [Required]
        public long AlphasCount { get; set; }

        [Required]
        public long DeltasCount { get; set; }

        [Required]
        public long CharliesCount { get; set; }

        [Required]
        public long MissesCount { get; set; }

        public string StageTimeInput { get; set; }

        [Required]
        public TimeSpan StageTime =>
        TimeSpan.FromSeconds(double.Parse(StageTimeInput.Replace(',', '.'), CultureInfo.InvariantCulture));
    }
}
