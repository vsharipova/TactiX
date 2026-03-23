using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TactiX.DBContext;
using TactiX.Models;

namespace TactiX.Controllers
{
    [Authorize]
    public class ExerciseLibraryController : Controller
    {
        private readonly TactiXDB _context;

        public ExerciseLibraryController(TactiXDB context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Exercises
                .Select(e => e.Category)
                .Distinct()
                .ToListAsync();

            var model = new ExerciseLibraryViewModel
            {
                Categories = categories,
                ExercisesByCategory = new Dictionary<ExerciseCategory, List<Exercise>>()
            };

            foreach (var category in categories)
            {
                var exercises = await _context.Exercises
                    .Where(e => e.Category == category)
                    .ToListAsync();
                model.ExercisesByCategory[category] = exercises;
            }

            return View(model);
        }

        public async Task<IActionResult> Category(ExerciseCategory category)
        {
            var exercises = await _context.Exercises
                .Where(e => e.Category == category)
                .ToListAsync();

            ViewBag.CategoryName = GetCategoryName(category);
            return View(exercises);
        }
        
        private string GetCategoryName(ExerciseCategory category)
        {
            return category switch
            {
                ExerciseCategory.Accuracy => "Точность",
                ExerciseCategory.Transfers => "Переносы",
                ExerciseCategory.DrawSpeed => "Скорость вскидки",
                ExerciseCategory.MovementSpeed => "Скорость перемещений",
                ExerciseCategory.ShootingOnMove => "Стрельба в движении",
                ExerciseCategory.Combination => "Комбинация",
                _ => category.ToString()
            };
        }
    }

    public class ExerciseLibraryViewModel
    {
        public List<ExerciseCategory> Categories { get; set; } = new();
        public Dictionary<ExerciseCategory, List<Exercise>> ExercisesByCategory { get; set; } = new();
    }
}