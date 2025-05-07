using Microsoft.EntityFrameworkCore;
using TactiX.Models;
using TactiX.DBContext;
using TactiX.Models.ViewModels;

namespace TactiX.Services
{
    public class TrainingAnalysisService
    {
        private readonly TactiXDB _context;
        private readonly ILogger<TrainingAnalysisService> _logger;

        public TrainingAnalysisService(TactiXDB context, ILogger<TrainingAnalysisService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TrainingAnalysisDto> AnalyzeTraining(int trainingId)
        {
            try
            {
                var training = await _context.Trainings
                    .Include(t => t.Stages)
                    .Include(t => t.Analysis)
                    .FirstOrDefaultAsync(t => t.TrainingId == trainingId);

                if (training == null) throw new Exception("Тренировка не найдена");

                var analysis = await CalculateTrainingAnalysis(training);
                _context.TrainingAnalyses.Add(analysis);
                await _context.SaveChangesAsync();

                return MapToDto(training, analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка анализа тренировки {TrainingId}", trainingId);
                throw;
            }
        }

        private async Task<TrainingAnalysis> CalculateTrainingAnalysis(Training training)
        {
            var totalShots = training.Stages.Sum(s => s.AlphasCount + s.DeltasCount + s.CharliesCount + s.MissesCount);

            var alphaPercent = totalShots > 0 ? (double)training.Stages.Sum(s => s.AlphasCount) / totalShots * 100 : 0;
            var charliePercent = totalShots > 0 ? (double)training.Stages.Sum(s => s.CharliesCount) / totalShots * 100 : 0;
            var deltaPercent = totalShots > 0 ? (double)training.Stages.Sum(s => s.DeltasCount) / totalShots * 100 : 0;
            var missPercent = totalShots > 0 ? (double)training.Stages.Sum(s => s.MissesCount) / totalShots * 100 : 0;

            var avgHitFactor = training.Stages.Any() ? training.Stages.Average(s => s.HitFactor) : 0;

            var bestTrainingAnalysis = await _context.TrainingAnalyses
                .OrderByDescending(ta => ta.PerformanceScore)
                .FirstOrDefaultAsync();

            var performanceScore = CalculatePerformanceScore(training, bestTrainingAnalysis);

            return new TrainingAnalysis
            {
                TrainingId = training.TrainingId,
                TrainingType = training.TypeOfTraining,
                TotalShots = (int)totalShots,
                TotalAlphas = (int)training.Stages.Sum(s => s.AlphasCount),
                TotalDeltas = (int)training.Stages.Sum(s => s.DeltasCount),
                TotalCharlies = (int)training.Stages.Sum(s => s.CharliesCount),
                TotalMisses = (int)training.Stages.Sum(s => s.MissesCount),
                AlphaPercentage = (decimal)alphaPercent,
                DeltaPercentage = (decimal)deltaPercent,
                CharliePercentage = (decimal)charliePercent,
                MissPercentage = (decimal)missPercent,
                AvgHitFactor = (decimal)avgHitFactor,
                PerformanceScore = performanceScore,
                IsBestPerformance = bestTrainingAnalysis == null || performanceScore >= bestTrainingAnalysis.PerformanceScore,
                CalculatedAt = DateTime.UtcNow
            };
        }

        private decimal CalculatePerformanceScore(Training training, TrainingAnalysis bestTrainingAnalysis)
        {
            if (!training.Stages.Any()) return 0;

            var currentScore = training.Stages.Sum(s =>
                s.AlphasCount * 5 +
                s.CharliesCount * 3 +
                s.DeltasCount * 1);

            if (bestTrainingAnalysis == null) return 10.0m;

            var bestScore = bestTrainingAnalysis.TotalAlphas * 5 +
                          bestTrainingAnalysis.TotalCharlies * 3 +
                          bestTrainingAnalysis.TotalDeltas * 1;

            if (bestScore == 0) return 10.0m;

            var score = (decimal)currentScore / bestScore * 10m;
            return Math.Min(10.0m, Math.Round(score, 1));
        }

        public async Task<TrainingComparisonDto> CompareTrainings(int baseTrainingId, int comparedTrainingId)
        {
            try
            {
                var baseTraining = await _context.Trainings
                    .Include(t => t.Analysis)
                    .FirstOrDefaultAsync(t => t.TrainingId == baseTrainingId);

                var comparedTraining = await _context.Trainings
                    .Include(t => t.Analysis)
                    .FirstOrDefaultAsync(t => t.TrainingId == comparedTrainingId);

                if (baseTraining == null || comparedTraining == null)
                    throw new Exception("Одна из тренировок не найдена");

                if (baseTraining.Analysis == null || comparedTraining.Analysis == null)
                    throw new Exception("Анализ для одной из тренировок отсутствует");

                return new TrainingComparisonDto
                {
                    BaseTrainingId = baseTrainingId,
                    ComparedTrainingId = comparedTrainingId,
                    ComparedTrainingDate = comparedTraining.TrainingDate,
                    AlphaDiff = baseTraining.Analysis.AlphaPercentage - comparedTraining.Analysis.AlphaPercentage,
                    CharlieDiff = baseTraining.Analysis.CharliePercentage - comparedTraining.Analysis.CharliePercentage,
                    DeltaDiff = baseTraining.Analysis.DeltaPercentage - comparedTraining.Analysis.DeltaPercentage,
                    MissDiff = baseTraining.Analysis.MissPercentage - comparedTraining.Analysis.MissPercentage,
                    HitFactorDiff = baseTraining.Analysis.AvgHitFactor - comparedTraining.Analysis.AvgHitFactor,
                    ComparisonResult = baseTraining.Analysis.PerformanceScore > comparedTraining.Analysis.PerformanceScore
                        ? "better"
                        : baseTraining.Analysis.PerformanceScore < comparedTraining.Analysis.PerformanceScore
                            ? "worse"
                            : "similar",
                    Advice = GenerateComparisonAdvice(
                        baseTraining.Analysis.AlphaPercentage - comparedTraining.Analysis.AlphaPercentage,
                        baseTraining.Analysis.DeltaPercentage - comparedTraining.Analysis.DeltaPercentage,
                        baseTraining.Analysis.CharliePercentage - comparedTraining.Analysis.CharliePercentage,
                        baseTraining.Analysis.MissPercentage - comparedTraining.Analysis.MissPercentage)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сравнении тренировок {BaseTrainingId} и {ComparedTrainingId}",
                    baseTrainingId, comparedTrainingId);
                throw;
            }
        }

        private TrainingAnalysisDto MapToDto(Training training, TrainingAnalysis analysis)
        {
            return new TrainingAnalysisDto
            {
                TrainingId = training.TrainingId,
                TrainingDate = training.TrainingDate,
                TrainingType = analysis.TrainingType,
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

        private string GenerateAdvice(TrainingAnalysis analysis)
        {
            var advice = new List<string>();

            if (analysis.MissPercentage > 15)
                advice.Add("Слишком много промахов - работайте над точностью");

            if (analysis.AlphaPercentage < 40)
                advice.Add("Низкий процент попаданий в Альфа-зону - тренируйте прицеливание");

            // Дополнительные советы по типу тренировки
            if (analysis.TrainingType == "Скорость" && analysis.AvgHitFactor < 5)
                advice.Add("Низкий Hit Factor - работайте над скоростью без потери точности");

            return advice.Count > 0 ? string.Join("; ", advice) : "Хороший результат! Продолжайте в том же духе";
        }
    }
}