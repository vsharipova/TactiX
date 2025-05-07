using System.ComponentModel.DataAnnotations;

namespace TactiX.Models
{
    public class SupportRequest
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Ваше имя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Выберите тип обращения")]
        [Display(Name = "Тип обращения")]
        public string MessageType { get; set; }

        [Required(ErrorMessage = "Введите текст сообщения")]
        [MinLength(10, ErrorMessage = "Минимум 10 символов")]
        [Display(Name = "Сообщение")]
        public string MessageText { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Необходимо согласие")]
        [Display(Name = "Согласие на обработку данных")]
        public bool AgreeTerms { get; set; }
    }
}