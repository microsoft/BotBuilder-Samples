using Catering.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Catering.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public async Task<IActionResult> Index()
        {
            var secret = Configuration.GetSection("ClientDirectLineSecret")?.Value;
            var endpoint = Configuration.GetSection("ClientDirectLineEndpoint")?.Value;

            var userId = "dl_" + Guid.NewGuid().ToString();
            
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/tokens/generate");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secret);
            request.Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    User = new { Id = userId }, 
                    TrustedOrigins = new string[] 
                    { 
                        "http://localhost:2978",
                        "https://jeffcateringbot.azurewebsites.net"
                    }
                }),
                Encoding.UTF8,
                "application/json");

            var response = await client.SendAsync(request);
            string token = String.Empty;

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<DirectLineToken>(body).token;
            }

            var config = new ChatConfig()
            {
                Token = token,
                Domain = endpoint,
                UserId = userId
                
            };
            return View(config);
        }
    }
}
