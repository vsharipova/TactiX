namespace TactiX.Models.ViewModels
{
    public class TrainingAnalysisDto
    {
        public int TrainingId { get; set; }
        public string TrainingName { get; set; }
        public DateTime TrainingDate { get; set; }
        public string TrainingType { get; set; } // "Точность", "Скорость" и т.д.

        public int TotalShots { get; set; }
        public decimal AlphaPercentage { get; set; }
        public decimal DeltaPercentage { get; set; }
        public decimal CharliePercentage { get; set; }
        public decimal MissPercentage { get; set; }
        public decimal AvgHitFactor { get; set; }
        public decimal PerformanceScore { get; set; }
        public bool IsBestPerformance { get; set; }
        public DateTime CalculatedAt { get; set; }
        public string ComparisonAdvice { get; set; }

        public List<TrainingComparisonDto> Comparisons { get; set; } = new();
    }

    public class TrainingComparisonDto
    {
        public int ComparedTrainingId { get; set; }
        public string ComparedTrainingName { get; set; }
        public int BaseTrainingId { get; set; }
        public DateTime ComparedTrainingDate { get; set; }
        public decimal AlphaDiff { get; set; }
        public decimal DeltaDiff { get; set; }
        public decimal CharlieDiff { get; set; }
        public decimal MissDiff { get; set; }
        public decimal HitFactorDiff { get; set; }
        public string ComparisonResult { get; set; }
        public string Advice { get; set; }
    }
}