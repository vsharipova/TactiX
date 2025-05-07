using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TactiX.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }

    public class ElasticEmailService : IEmailService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ElasticEmailService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var requestData = new
            {
                apiKey = _config["ElasticEmail:ApiKey"],
                from = _config["ElasticEmail:FromEmail"],
                fromName = _config["ElasticEmail:FromName"],
                to = toEmail,
                subject = subject,
                bodyHtml = body,  
                isTransactional = true
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(requestData),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await httpClient.PostAsync(
                    "https://api.elasticemail.com/v2/email/send",
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ElasticEmail Response: {responseContent}"); 

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                dynamic result = JsonConvert.DeserializeObject(responseContent);
                if (result.success == false)
                {
                    Console.WriteLine($"ElasticEmail error: {result.error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return false;
            }
        }
    }
}