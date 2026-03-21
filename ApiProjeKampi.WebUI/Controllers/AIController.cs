using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ApiProjeKampi.WebUI.Controllers
{
    public class AIController : Controller
    {
        public IActionResult CreateRecipeWithGroqAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipeWithGroqAI(string prompt)
        {
            https://console.groq.com/keys
            var apiKey = ""; 

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                model = "llama-3.3-70b-versatile", 
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Sen bir restoran için yemek önerilerini yapan bir yapay zeka aracısın. Amacımız kullanıcı tarafından girilen malzemelere göre yemek tarifi önerisinde bulunmak. Lütfen tarifleri HTML formatında (<b>, <ul>, <li> kullanarak) döndür."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.5
            };

            var response = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GroqAIResponse>();

                var content = result.choices[0].message.content;
                ViewBag.recipe = content;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.recipe = "Bir hata oluştu: " + response.StatusCode + " - " + error;
            }

            return View();
        }

        public class GroqAIResponse
        {
            public List<Choice> choices { get; set; }
        }

        public class Choice
        {
            public Message message { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}