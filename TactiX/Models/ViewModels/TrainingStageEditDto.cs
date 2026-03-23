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
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int NumOfSpots { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int NumOfPoppers { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int NumOfPlates { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int AlphasCount { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int DeltasCount { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int CharliesCount { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        public int MissesCount { get; set; }

        public TimeSpan StageTime { get; set; }
        public string StageTimeInput
        {
            get => StageTime.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
            set => StageTime = TimeSpan.FromSeconds(double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture));
        }
    }
}
