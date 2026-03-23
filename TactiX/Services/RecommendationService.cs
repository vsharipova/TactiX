using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TactiX.DBContext;
using TactiX.Models;

namespace TactiX.Services
{
    public class RecommendationService
    {
        private readonly TactiXDB _context;

        public RecommendationService(TactiXDB context)
        {
            _context = context;
        }

        public async Task<PersonalRecommendation> GenerateRecommendationsAsync(int userId)
        {
            var lastMatch = await _context.Matches
                .Include(m => m.Stages)
                .Where(m => m.UserId == userId && m.MatchDate < DateTime.Now)
                .OrderByDescending(m => m.MatchDate)
                .FirstOrDefaultAsync();

            if (lastMatch == null)
            {
                return new PersonalRecommendation
                {
                    HasData = false,
                    Message = "У вас пока нет завершенных матчей для анализа."
                };
            }

            var recentMatches = await _context.Matches
                .Include(m => m.Stages)
                .Where(m => m.UserId == userId && m.MatchDate < DateTime.Now)
                .OrderByDescending(m => m.MatchDate)
                .Take(10)
                .ToListAsync();

            if (recentMatches.Count < 2)
            {
                var prevMatches = recentMatches.Where(m => m.MatchId != lastMatch.MatchId).ToList();
                var lastAcc = CalculateAccuracy(lastMatch);
                var prevAcc = prevMatches.Any() ? CalculateAverageAccuracy(prevMatches) : lastAcc;
                var lastSpd = CalculateSpeed(lastMatch);
                var prevSpd = prevMatches.Any() ? CalculateAverageSpeed(prevMatches) : lastSpd;

                var accChange = lastAcc - prevAcc;
                var spdChangePercent = CalculatePercentageChange(lastSpd, prevSpd);

                var recommendation = BuildRecommendation(lastAcc, prevAcc, accChange,
                                                         lastSpd, prevSpd, spdChangePercent);
                var exercises = await SelectExercisesAsync(accChange, spdChangePercent);

                return new PersonalRecommendation
                {
                    HasData = true,
                    LastMatch = lastMatch,
                    PreviousMatches = prevMatches,
                    AccuracyChange = accChange,
                    SpeedChange = spdChangePercent,
                    AccuracyIssue = null,
                    SpeedIssue = null,
                    AccuracyResidual = null,
                    SpeedResidual = null,
                    AccuracySlope = null,
                    SpeedSlope = null,
                    RecommendationMessage = recommendation,
                    SuggestedExercises = exercises
                };
            }

            var matchesOrdered = recentMatches.OrderBy(m => m.MatchDate).ToList(); 
            var accuracies = matchesOrdered.Select(m => (double)CalculateAccuracy(m)).ToList();
            var speeds = matchesOrdered.Select(m => (double)CalculateSpeed(m)).ToList();
            var indices = Enumerable.Range(0, matchesOrdered.Count).Select(i => (double)i).ToList();

            var (accuracyIssue, speedIssue, accResidual, speedResidual, accSlope, speedSlope) =
                DetectIssuesDetailed(indices, accuracies, speeds);

            var previousMatches = recentMatches.Where(m => m.MatchId != lastMatch.MatchId).Take(3).ToList();
            var lastMatchAccuracy = CalculateAccuracy(lastMatch);
            var previousAccuracy = previousMatches.Any() ? CalculateAverageAccuracy(previousMatches) : lastMatchAccuracy;
            var lastMatchSpeed = CalculateSpeed(lastMatch);
            var previousSpeed = previousMatches.Any() ? CalculateAverageSpeed(previousMatches) : lastMatchSpeed;

            var accuracyChange = lastMatchAccuracy - previousAccuracy;
            var speedChangePercent = CalculatePercentageChange(lastMatchSpeed, previousSpeed);

            bool accuracyProblem = accuracyIssue || accuracyChange < -5;
            bool speedProblem = speedIssue || speedChangePercent > 5;

            var mlRecommendation = BuildMLRecommendation(accuracyIssue, speedIssue, accuracyChange, speedChangePercent);

            var mlExercises = await SelectExercisesMLDetailedAsync(
                accuracyProblem, speedProblem, accResidual, speedResidual, accSlope, speedSlope, userId);

            return new PersonalRecommendation
            {
                HasData = true,
                LastMatch = lastMatch,
                PreviousMatches = previousMatches,
                AccuracyChange = accuracyChange,
                SpeedChange = speedChangePercent,
                AccuracyIssue = accuracyIssue,
                SpeedIssue = speedIssue,
                AccuracyResidual = accResidual,
                SpeedResidual = speedResidual,
                AccuracySlope = accSlope,
                SpeedSlope = speedSlope,
                RecommendationMessage = mlRecommendation,
                SuggestedExercises = mlExercises
            };
        }

        private decimal CalculateAccuracy(Match match)
        {
            var totalShots = match.Stages.Sum(s => s.AlphasCount + s.CharliesCount + s.DeltasCount + s.MissesCount);
            if (totalShots == 0) return 0;
            var hits = match.Stages.Sum(s => s.AlphasCount + s.CharliesCount + s.DeltasCount);
            return (decimal)hits / totalShots * 100;
        }

        private decimal CalculateAverageAccuracy(List<Match> matches)
        {
            if (!matches.Any()) return 0;
            return matches.Average(m => CalculateAccuracy(m));
        }

        private decimal CalculateSpeed(Match match)
        {
            if (!match.Stages.Any()) return 0;
            return (decimal)match.Stages.Average(s => s.StageTime.TotalSeconds);
        }

        private decimal CalculateAverageSpeed(List<Match> matches)
        {
            if (!matches.Any()) return 0;
            return matches.Average(m => CalculateSpeed(m));
        }

        private decimal CalculatePercentageChange(decimal current, decimal previous)
        {
            if (previous == 0) return 0;
            return (current - previous) / previous * 100;
        }

        private string BuildRecommendation(decimal lastAccuracy, decimal prevAccuracy, decimal accuracyChange,
                                           decimal lastSpeed, decimal prevSpeed, decimal speedChange)
        {
            var messages = new List<string>();

            if (accuracyChange < -5) 
            {
                messages.Add($"На последнем матче ваша точность снизилась на {Math.Abs(accuracyChange):F1}%.");
            }
            else if (accuracyChange > 5)
            {
                messages.Add($"На последнем матче ваша точность улучшилась на {accuracyChange:F1}%.");
            }

            if (speedChange > 5) 
            {
                messages.Add($"Среднее время прохождения упражнений увеличилось на {speedChange:F1}%.");
            }
            else if (speedChange < -5)
            {
                messages.Add($"Среднее время прохождения упражнений сократилось на {Math.Abs(speedChange):F1}%.");
            }

            if (!messages.Any())
                return "Ваши показатели стабильны. Продолжайте в том же духе!";

            return string.Join(" ", messages);
        }

        private async Task<List<Exercise>> SelectExercisesAsync(decimal accuracyChange, decimal speedChange)
        {
            var query = _context.Exercises.AsQueryable();

            var categories = new List<ExerciseCategory>();
            if (accuracyChange < -5) 
            {
                categories.Add(ExerciseCategory.Accuracy);
                categories.Add(ExerciseCategory.Transfers);
            }
            if (speedChange > 5) 
            {
                categories.Add(ExerciseCategory.DrawSpeed);
                categories.Add(ExerciseCategory.MovementSpeed);
                categories.Add(ExerciseCategory.ShootingOnMove);
            }

            if (!categories.Any())
                categories = Enum.GetValues<ExerciseCategory>().ToList(); 

            query = query.Where(e => categories.Contains(e.Category))
                         .OrderBy(r => EF.Functions.Random()) 
                         .Take(3);

            return await query.ToListAsync();
        }

        private (double slope, double intercept) FitLinearRegression(List<double> x, List<double> y)
        {
            if (x.Count != y.Count || x.Count < 2)
                return (0, y.Any() ? y.Average() : 0);

            double sumX = x.Sum();
            double sumY = y.Sum();
            double sumXY = x.Zip(y, (a, b) => a * b).Sum();
            double sumX2 = x.Sum(a => a * a);
            double n = x.Count;

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;

            return (slope, intercept);
        }

        private (bool accuracyIssue, bool speedIssue, double accResidual, double speedResidual, double accSlope, double speedSlope) DetectIssuesDetailed(List<double> indices, List<double> accuracies, List<double> speeds)
        {
            var (accSlope, accIntercept) = FitLinearRegression(indices, accuracies);
 
            var predictedAcc = indices.Select(i => accSlope * i + accIntercept).ToList();
            var residualsAcc = accuracies.Zip(predictedAcc, (a, p) => a - p).ToList();
            double stdAcc = Math.Sqrt(residualsAcc.Select(r => r * r).Sum() / (residualsAcc.Count - 1));
 
            double lastResidualAcc = residualsAcc.Last();
            bool accuracyIssue = lastResidualAcc < -1.5 * stdAcc; 

            var (speedSlope, speedIntercept) = FitLinearRegression(indices, speeds);
            var predictedSpeed = indices.Select(i => speedSlope * i + speedIntercept).ToList();
            var residualsSpeed = speeds.Zip(predictedSpeed, (s, p) => s - p).ToList();
            double stdSpeed = Math.Sqrt(residualsSpeed.Select(r => r * r).Sum() / (residualsSpeed.Count - 1));
            double lastResidualSpeed = residualsSpeed.Last();
            bool speedIssue = lastResidualSpeed > 1.5 * stdSpeed; 

            return (accuracyIssue, speedIssue, lastResidualAcc, lastResidualSpeed, accSlope, speedSlope);
        }

        private (bool accuracyIssue, bool speedIssue) DetectIssues(List<double> indices, List<double> accuracies, List<double> speeds)
        {
            var (accuracyIssue, speedIssue, _, _, _, _) = DetectIssuesDetailed(indices, accuracies, speeds);
            return (accuracyIssue, speedIssue);
        }

        private string BuildMLRecommendation(bool accuracyIssue, bool speedIssue, decimal accuracyChange, decimal speedChange)
        {
            var messages = new List<string>();

            bool accuracyProblem = accuracyIssue || accuracyChange < -5;
            bool speedProblem = speedIssue || speedChange > 5;

            if (accuracyProblem && speedProblem)
            {
                messages.Add("Показатели значительно ухудшились.");
            }

            if (accuracyIssue)
            {
                messages.Add("Анализ машинного обучения выявил снижение точности по сравнению с ожидаемым трендом.");
                if (accuracyChange < -5)
                    messages.Add($"На последнем матче ваша точность снизилась на {Math.Abs(accuracyChange):F1}%.");
            }
            else
            {
                if (accuracyChange < -5)
                    messages.Add($"На последнем матче ваша точность снизилась на {Math.Abs(accuracyChange):F1}%.");
                else if (accuracyChange > 5)
                    messages.Add($"На последнем матче ваша точность улучшилась на {accuracyChange:F1}%.");
            }

            if (speedIssue)
            {
                messages.Add("Анализ машинного обучения выявил увеличение времени прохождения упражнений.");
                if (speedChange > 5)
                    messages.Add($"Среднее время прохождения упражнений увеличилось на {speedChange:F1}%.");
            }
            else
            {
                if (speedChange > 5)
                    messages.Add($"Среднее время прохождения упражнений увеличилось на {speedChange:F1}%.");
                else if (speedChange < -5)
                    messages.Add($"Среднее время прохождения упражнений сократилось на {Math.Abs(speedChange):F1}%.");
            }

            if (!messages.Any())
                return "Ваши показатели соответствуют ожидаемому тренду. Продолжайте в том же духе!";

            return string.Join(" ", messages);
        }

        private async Task<List<Exercise>> SelectExercisesMLAsync(bool accuracyIssue, bool speedIssue)
        {
            var query = _context.Exercises.AsQueryable();

            var categories = new List<ExerciseCategory>();
            if (accuracyIssue)
            {
                categories.Add(ExerciseCategory.Accuracy);
                categories.Add(ExerciseCategory.Transfers);
            }
            if (speedIssue)
            {
                categories.Add(ExerciseCategory.DrawSpeed);
                categories.Add(ExerciseCategory.MovementSpeed);
                categories.Add(ExerciseCategory.ShootingOnMove);
            }

            if (!categories.Any())
                categories = Enum.GetValues<ExerciseCategory>().ToList(); 

            query = query.Where(e => categories.Contains(e.Category))
                         .OrderBy(r => EF.Functions.Random())
                         .Take(3);

            return await query.ToListAsync();
        }

        private async Task<List<Exercise>> SelectExercisesMLDetailedAsync(
            bool accuracyIssue, bool speedIssue,
            double accResidual, double speedResidual,
            double accSlope, double speedSlope,
            int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var practiceLevel = user?.PracticeLevel ?? "Средний";
            DifficultyLevel targetDifficulty = MapPracticeLevelToDifficulty(practiceLevel);

            var exercises = await _context.Exercises.ToListAsync();

            var scoredExercises = exercises.Select(e => new
            {
                Exercise = e,
                Score = ComputeRelevanceScore(e, accuracyIssue, speedIssue, accResidual, speedResidual, accSlope, speedSlope, targetDifficulty)
            }).ToList();

            var topExercises = scoredExercises
                .OrderByDescending(se => se.Score)
                .Take(3)
                .Select(se => se.Exercise)
                .ToList();

            if (topExercises.Count < 3)
            {
                var fallback = await SelectExercisesMLAsync(accuracyIssue, speedIssue);
                topExercises.AddRange(fallback.Take(3 - topExercises.Count));
                topExercises = topExercises.Distinct().Take(3).ToList();
            }

            return topExercises;
        }

        private DifficultyLevel MapPracticeLevelToDifficulty(string practiceLevel)
        {
            return practiceLevel.ToLower() switch
            {
                "начинающий" => DifficultyLevel.Beginner,
                "средний" => DifficultyLevel.Intermediate,
                "продвинутый" => DifficultyLevel.Advanced,
                _ => DifficultyLevel.Intermediate
            };
        }

        private double ComputeRelevanceScore(Exercise exercise,
            bool accuracyIssue, bool speedIssue,
            double accResidual, double speedResidual,
            double accSlope, double speedSlope,
            DifficultyLevel targetDifficulty)
        {
            double score = 0.0;

            if (accuracyIssue && (exercise.Category == ExerciseCategory.Accuracy || exercise.Category == ExerciseCategory.Transfers))
                score += 2.0;
            if (speedIssue && (exercise.Category == ExerciseCategory.DrawSpeed || exercise.Category == ExerciseCategory.MovementSpeed || exercise.Category == ExerciseCategory.ShootingOnMove))
                score += 2.0;

            if (accuracyIssue)
                score += Math.Abs(accResidual) * 0.1; 
            if (speedIssue)
                score += Math.Abs(speedResidual) * 0.1;

            if (accSlope < 0) 
                score += Math.Abs(accSlope) * 10; 
            if (speedSlope > 0) 
                score += Math.Abs(speedSlope) * 10;

            var diffDiff = Math.Abs((int)exercise.Difficulty - (int)targetDifficulty);
            if (diffDiff == 0)
                score += 1.0;
            else if (diffDiff == 1)
                score += 0.5;

            score += new Random().NextDouble() * 0.01;

            return score;
        }
    }

    public class PersonalRecommendation
    {
        public bool HasData { get; set; }
        public string? Message { get; set; }
        public Match? LastMatch { get; set; }
        public List<Match>? PreviousMatches { get; set; }
        public decimal AccuracyChange { get; set; }
        public decimal SpeedChange { get; set; }
        public bool? AccuracyIssue { get; set; }
        public bool? SpeedIssue { get; set; }
        public double? AccuracyResidual { get; set; }
        public double? SpeedResidual { get; set; }
        public double? AccuracySlope { get; set; }
        public double? SpeedSlope { get; set; }
        public string RecommendationMessage { get; set; } = "";
        public List<Exercise> SuggestedExercises { get; set; } = new();
    }
}