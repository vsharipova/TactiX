using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TactiX.Models.ViewModels
{
    public class MatchStageEditDto
    {
        public int MatchStageId { get; set; }
        public int MatchId { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(255)]
        public string StageName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string StageType { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0000}")]
        public double HitFactor { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int NumOfSpots { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int NumOfPoppers { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int NumOfPlates { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int AlphasCount { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int DeltasCount { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int CharliesCount { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int MissesCount { get; set; }

        public TimeSpan StageTime { get; set; }
        public string StageTimeInput
        {
            get => StageTime.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
            set => StageTime = TimeSpan.FromSeconds(double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture));
        }
    }
}