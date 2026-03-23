using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TactiX.Services;

namespace TactiX.Controllers
{
    [Authorize]
    public class PersonalRecommendationsController : Controller
    {
        private readonly RecommendationService _recommendationService;

        public PersonalRecommendationsController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var recommendation = await _recommendationService.GenerateRecommendationsAsync(userId);
            return View(recommendation);
        }
    }
}