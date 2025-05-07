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
        public long StageType { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0000}")]
        public double HitFactor { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long NumOfSpots { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long NumOfPoppers { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long NumOfPlates { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long AlphasCount { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long DeltasCount { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long CharliesCount { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public long MissesCount { get; set; }

        public TimeSpan StageTime { get; set; }
        public string StageTimeInput
        {
            get => StageTime.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
            set => StageTime = TimeSpan.FromSeconds(double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture));
        }
    }
}