using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TactiX.DBContext;
using TactiX.Models.ViewModels;

namespace TactiX.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly TactiXDB _context;

        public HomeController(TactiXDB context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var model = new HomeViewModel
            {
                NextMatch = _context.Matches
                    .Where(m => m.UserId == userId && m.MatchDate >= DateTime.Today)
                    .OrderBy(m => m.MatchDate)
                    .FirstOrDefault(),

                MonthlyStats = CalculateMonthlyStats(userId, DateTime.Today.Month),

                PrevMonthStats = CalculateMonthlyStats(userId, DateTime.Today.AddMonths(-1).Month),
            };

            return View(model);
        }

        private MonthlyStats CalculateMonthlyStats(int userId, int month)
        {
            var stats = new MonthlyStats();

            try
            {
                var firstDayOfMonth = new DateTime(DateTime.Today.Year, month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var matchStats = _context.Matches
                    .Where(m => m.UserId == userId &&
                               m.MatchDate >= firstDayOfMonth &&
                               m.MatchDate <= lastDayOfMonth)
                    .SelectMany(m => m.Stages)
                    .GroupBy(s => 1)
                    .Select(g => new {
                        Alphas = g.Sum(s => s.AlphasCount),
                        Charlies = g.Sum(s => s.CharliesCount),
                        Deltas = g.Sum(s => s.DeltasCount),
                        Misses = g.Sum(s => s.MissesCount)
                    })
                    .FirstOrDefault();

                var trainingStats = _context.Trainings
                    .Where(t => t.UserId == userId &&
                               t.TrainingDate >= firstDayOfMonth &&
                               t.TrainingDate <= lastDayOfMonth)
                    .SelectMany(t => t.Stages)
                    .GroupBy(s => 1)
                    .Select(g => new {
                        Alphas = g.Sum(s => s.AlphasCount),
                        Charlies = g.Sum(s => s.CharliesCount),
                        Deltas = g.Sum(s => s.DeltasCount),
                        Misses = g.Sum(s => s.MissesCount)
                    })
                    .FirstOrDefault();

                int totalAlphas = (int)(matchStats?.Alphas ?? 0) + (int)(trainingStats?.Alphas ?? 0);
                int totalCharlies = (int)(matchStats?.Charlies ?? 0) + (int)(trainingStats?.Charlies ?? 0);
                int totalDeltas = (int)(matchStats?.Deltas ?? 0) + (int)(trainingStats?.Deltas ?? 0);
                int totalMisses = (int)(matchStats?.Misses ?? 0) + (int)(trainingStats?.Misses ?? 0);
                int totalShots = totalAlphas + totalCharlies + totalDeltas + totalMisses;

                if (totalShots > 0)
                {
                    stats.AlphaPercentage = Math.Round((decimal)totalAlphas / totalShots * 100, 1);
                    stats.CharliePercentage = Math.Round((decimal)totalCharlies / totalShots * 100, 1);
                    stats.DeltaPercentage = Math.Round((decimal)totalDeltas / totalShots * 100, 1);
                    stats.MissPercentage = Math.Round((decimal)totalMisses / totalShots * 100, 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при расчете статистики: {ex.Message}");
            }

            return stats;
        }

        public IActionResult TrainingCalendar()
        {
            return View();
        }

        public IActionResult MatchCalendar()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult TrainingAnalysis()
        {
            return View();
        }

        public IActionResult MatchAnalysis()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}