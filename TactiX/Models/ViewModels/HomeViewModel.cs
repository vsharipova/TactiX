namespace TactiX.Models.ViewModels
{
    public class HomeViewModel
    {
        public Match NextMatch { get; set; }
        public MonthlyStats MonthlyStats { get; set; }
        public MonthlyStats PrevMonthStats { get; set; }
        public List<string> TrainingAdvices { get; set; }
    }

    public class MonthlyStats
    {
        public decimal AlphaPercentage { get; set; }
        public decimal CharliePercentage { get; set; }
        public decimal DeltaPercentage { get; set; }
        public decimal MissPercentage { get; set; }
    }
}
