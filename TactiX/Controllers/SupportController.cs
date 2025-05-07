using Microsoft.AspNetCore.Mvc;
using TactiX.Models;
using TactiX.Services;

namespace TactiX.Controllers
{
    public class SupportController : Controller
    {
        private readonly IEmailService _emailService;

        public SupportController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new SupportRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendSupportRequest(SupportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", request);
            }

            try
            {
                var emailBody = $"<h3>Новое обращение в поддержку</h3>" +
                              $"<p><strong>Имя:</strong> {request.UserName}</p>" +
                              $"<p><strong>Email:</strong> {request.UserEmail}</p>" +
                              $"<p><strong>Тип:</strong> {request.MessageType}</p>" +
                              $"<p><strong>Сообщение:</strong></p><p>{request.MessageText}</p>";

                Console.WriteLine($"Попытка отправки письма на {request.UserEmail}");

                var isSent = await _emailService.SendEmailAsync(
                    "vikulina059@gmail.com",
                    $"Обращение в поддержку: {request.MessageType}",
                    emailBody);

                if (isSent)
                {
                    Console.WriteLine("Письмо успешно отправлено");
                    TempData["SuccessMessage"] = "Ваше сообщение успешно отправлено! Мы ответим в ближайшее время.";
                }
                else
                {
                    Console.WriteLine("Ошибка при отправке письма");
                    TempData["ErrorMessage"] = "Ошибка при отправке сообщения. Пожалуйста, попробуйте позже.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex}");
                TempData["ErrorMessage"] = "Произошла системная ошибка. Пожалуйста, напишите нам напрямую на tactix.help@rambler.ru";
            }

            return RedirectToAction("Index");
        }
    }
}