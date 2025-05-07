using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TactiX.DBContext;
using TactiX.Models;

namespace TactiX.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly TactiXDB _context;

        public ProfileController(TactiXDB context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Surname = updatedUser.Surname;
                user.Email = updatedUser.Email;
                user.PracticeLevel = updatedUser.PracticeLevel;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Профиль успешно обновлен";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.HashPassword))
                {
                    TempData["ErrorMessage"] = "Текущий пароль неверен";
                    return RedirectToAction("Index");
                }

                user.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Пароль успешно изменен";
            }

            return RedirectToAction("Index");
        }
    }
}