using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TactiX.Models.ViewModels
{
    public class TrainingStageEditDto
    {
        public int TrainingStageId { get; set; }
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

        public TimeSpan StageTime { get; set; }
        public string StageTimeInput
        {
            get => StageTime.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
            set => StageTime = TimeSpan.FromSeconds(double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture));
        }
    }
}
