using Microsoft.EntityFrameworkCore;
using TactiX.Models;
using TactiX.DBContext;
using TactiX.Models.ViewModels;

namespace TactiX.Services
{
    public class MatchAnalysisService
    {
        private readonly TactiXDB _context;
        private readonly ILogger<MatchAnalysisService> _logger;

        public MatchAnalysisService(TactiXDB context, ILogger<MatchAnalysisService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MatchAnalysisDto> AnalyzeMatch(int matchId)
        {
            try
            {
                var match = await _context.Matches
                    .Include(m => m.Stages)
                    .Include(m => m.Analysis)
                    .FirstOrDefaultAsync(m => m.MatchId == matchId);

                if (match == null) throw new Exception("Матч не найден");

                var analysis = await CalculateMatchAnalysis(match);

                _context.MatchAnalyses.Add(analysis);
                await _context.SaveChangesAsync();

                return MapToDto(match, analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка анализа матча {MatchId}", matchId);
                throw;
            }
        }

        private async Task<MatchAnalysis> CalculateMatchAnalysis(Match match)
        {
            var totalShots = match.Stages.Sum(s => s.AlphasCount + s.DeltasCount + s.CharliesCount + s.MissesCount);

            var alphaPercent = totalShots > 0 ? (double)match.Stages.Sum(s => s.AlphasCount) / totalShots * 100 : 0;
            var charliePercent = totalShots > 0 ? (double)match.Stages.Sum(s => s.CharliesCount) / totalShots * 100 : 0;
            var deltaPercent = totalShots > 0 ? (double)match.Stages.Sum(s => s.DeltasCount) / totalShots * 100 : 0;
            var missPercent = totalShots > 0 ? (double)match.Stages.Sum(s => s.MissesCount) / totalShots * 100 : 0;

            var avgHitFactor = match.Stages.Any() ? match.Stages.Average(s => s.HitFactor) : 0;

            var bestMatchAnalysis = await _context.MatchAnalyses
                .OrderByDescending(ma => ma.PerformanceScore)
                .FirstOrDefaultAsync();

            var performanceScore = CalculatePerformanceScore(match, bestMatchAnalysis);

            return new MatchAnalysis
            {
                MatchId = match.MatchId,
                TotalShots = (int)totalShots,
                TotalAlphas = (int)match.Stages.Sum(s => s.AlphasCount),
                TotalDeltas = (int)match.Stages.Sum(s => s.DeltasCount),
                TotalCharlies = (int)match.Stages.Sum(s => s.CharliesCount),
                TotalMisses = (int)match.Stages.Sum(s => s.MissesCount),
                AlphaPercentage = (decimal)alphaPercent,
                DeltaPercentage = (decimal)deltaPercent,
                CharliePercentage = (decimal)charliePercent,
                MissPercentage = (decimal)missPercent,
                AvgHitFactor = (decimal)avgHitFactor,
                PerformanceScore = performanceScore,
                IsBestPerformance = bestMatchAnalysis == null || performanceScore >= bestMatchAnalysis.PerformanceScore,
                CalculatedAt = DateTime.UtcNow
            };
        }

        private decimal CalculatePerformanceScore(Match match, MatchAnalysis bestMatchAnalysis)
        {
            if (!match.Stages.Any()) return 0;

            var currentScore = match.Stages.Sum(s =>
                s.AlphasCount * 5 +
                s.CharliesCount * 3 +
                s.DeltasCount * 1);

            if (bestMatchAnalysis == null) return 10.0m;

            var bestScore = bestMatchAnalysis.TotalAlphas * 5 +
                          bestMatchAnalysis.TotalCharlies * 3 +
                          bestMatchAnalysis.TotalDeltas * 1;

            if (bestScore == 0) return 10.0m;

            var score = (decimal)currentScore / bestScore * 10m;
            return Math.Min(10.0m, Math.Round(score, 1));
        }

        public async Task<ComparisonDto> CompareMatches(int baseMatchId, int comparedMatchId)
        {
            try
            {
                var baseMatch = await _context.Matches
                    .Include(m => m.Analysis)
                    .FirstOrDefaultAsync(m => m.MatchId == baseMatchId);

                var comparedMatch = await _context.Matches
                    .Include(m => m.Analysis)
                    .FirstOrDefaultAsync(m => m.MatchId == comparedMatchId);

                if (baseMatch == null || comparedMatch == null)
                    throw new Exception("Один из матчей не найден");

                if (baseMatch.Analysis == null || comparedMatch.Analysis == null)
                    throw new Exception("Анализ для одного из матчей отсутствует");

                return new ComparisonDto
                {
                    BaseMatchId = baseMatchId,
                    ComparedMatchId = comparedMatchId,
                    ComparedMatchName = comparedMatch.MatchName ?? "Неизвестный матч",
                    ComparedMatchDate = comparedMatch.MatchDate,
                    AlphaDiff = baseMatch.Analysis.AlphaPercentage - comparedMatch.Analysis.AlphaPercentage,
                    CharlieDiff = baseMatch.Analysis.CharliePercentage - comparedMatch.Analysis.CharliePercentage,
                    DeltaDiff = baseMatch.Analysis.DeltaPercentage - comparedMatch.Analysis.DeltaPercentage,
                    MissDiff = baseMatch.Analysis.MissPercentage - comparedMatch.Analysis.MissPercentage,
                    HitFactorDiff = baseMatch.Analysis.AvgHitFactor - comparedMatch.Analysis.AvgHitFactor,
                    ComparisonResult = baseMatch.Analysis.PerformanceScore > comparedMatch.Analysis.PerformanceScore
                        ? "better"
                        : baseMatch.Analysis.PerformanceScore < comparedMatch.Analysis.PerformanceScore
                            ? "worse"
                            : "similar",
                    Advice = GenerateComparisonAdvice(
                        baseMatch.Analysis.AlphaPercentage - comparedMatch.Analysis.AlphaPercentage,
                        baseMatch.Analysis.DeltaPercentage - comparedMatch.Analysis.DeltaPercentage,
                        baseMatch.Analysis.CharliePercentage - comparedMatch.Analysis.CharliePercentage,
                        baseMatch.Analysis.MissPercentage - comparedMatch.Analysis.MissPercentage)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сравнении матчей {BaseMatchId} и {ComparedMatchId}",
                    baseMatchId, comparedMatchId);
                throw;
            }
        }

        private MatchAnalysisDto MapToDto(Match match, MatchAnalysis analysis)
        {
            return new MatchAnalysisDto
            {
                MatchId = match.MatchId,
                MatchName = match.MatchName,
                MatchDate = match.MatchDate,
                TotalShots = analysis.TotalShots,
                AlphaPercentage = analysis.AlphaPercentage,
                DeltaPercentage = analysis.DeltaPercentage,
                CharliePercentage = analysis.CharliePercentage,
                MissPercentage = analysis.MissPercentage,
                AvgHitFactor = analysis.AvgHitFactor,
                PerformanceScore = analysis.PerformanceScore,
                IsBestPerformance = analysis.IsBestPerformance,
                CalculatedAt = analysis.CalculatedAt,
                ComparisonAdvice = GenerateAdvice(analysis)
            };
        }

        private string GenerateComparisonAdvice(decimal alphaDiff, decimal deltaDiff,
                                     decimal charlieDiff, decimal missDiff)
        {
            var advice = new List<string>();

            if (alphaDiff > 0)
                advice.Add($"↑ Альфа на {Math.Abs(alphaDiff):0.0}% лучше");
            else if (alphaDiff < 0)
                advice.Add($"↓ Альфа на {Math.Abs(alphaDiff):0.0}% хуже");

            if (charlieDiff > 0)
                advice.Add($"↑ Чарли на {Math.Abs(charlieDiff):0.0}% лучше");
            else if (charlieDiff < 0)
                advice.Add($"↓ Чарли на {Math.Abs(charlieDiff):0.0}% хуже");

            if (deltaDiff > 0)
                advice.Add($"↑ Дельта на {Math.Abs(deltaDiff):0.0}% лучше");
            else if (deltaDiff < 0)
                advice.Add($"↓ Дельта на {Math.Abs(deltaDiff):0.0}% хуже");

            if (missDiff > 0)
                advice.Add($"↑ Промахов на {Math.Abs(missDiff):0.0}% больше");
            else if (missDiff < 0)
                advice.Add($"↓ Промахов на {Math.Abs(missDiff):0.0}% меньше");

            return advice.Count > 0 ? string.Join(", ", advice) : "Показатели идентичны";
        }

        private string GenerateAdvice(MatchAnalysis analysis)
        {
            var advice = new List<string>();

            if (analysis.MissPercentage > 15)
                advice.Add("Слишком много промахов - работайте над точностью");

            if (analysis.AlphaPercentage < 40)
                advice.Add("Низкий процент попаданий в Альфа-зону - тренируйте прицеливание");

            return advice.Count > 0 ? string.Join("; ", advice) : "Хороший результат! Продолжайте в том же духе";
        }
    }
}