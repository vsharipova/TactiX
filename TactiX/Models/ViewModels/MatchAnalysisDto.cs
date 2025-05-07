namespace TactiX.Models.ViewModels
{
    public class MatchAnalysisDto
    {
        public int MatchId { get; set; }

        public string MatchName { get; set; }
        public DateTime MatchDate { get; set; }

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

        public List<ComparisonDto> Comparisons { get; set; } = new();
    }

    public class ComparisonDto
    {
        public int ComparedMatchId { get; set; }
        public string ComparedMatchName { get; set; }
        public int BaseMatchId { get; set; }
        public DateTime ComparedMatchDate { get; set; }
        public decimal AlphaDiff { get; set; }
        public decimal DeltaDiff { get; set; }
        public decimal CharlieDiff { get; set; }
        public decimal MissDiff { get; set; }
        public decimal HitFactorDiff { get; set; }
        public string ComparisonResult { get; set; }
        public string Advice { get; set; }
    }
}
