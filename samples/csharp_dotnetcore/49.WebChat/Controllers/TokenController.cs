using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    [Route("directline/token")]
    [ApiController]
    /// <summary>
    /// This ASP Controller performs a token request from direct line using your Web Chat secret.   
    /// </summary>
    /// <remarks>
    /// Your client code must provide either a secret or a token to talk to your bot.
    /// Tokens are more secure, and this is the approach being demonstrated in this sample app. 
    /// To learn about the differences between secrets and tokens and to understand the risks associated with using secrets, visit 
    /// https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0
    /// </remarks>
    public class TokenController : ControllerBase
    {
        // The URL to the Direct Line token endpoint (where the token is requested from)
        const string TokenGenerationUrl = "https://directline.botframework.com/v3/directline/tokens/generate";
        private IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<DirectLineToken> GetAsync()
        {
            // The userId must always have ‘dl_’ pre-pended to it, which is a requirement for all Direct Line IDs.
            // All user IDs should be unique, if not multiple users could potentially share the same conversation state.
            string userId = $"dl_{Guid.NewGuid().ToString()}";

            using (var request = new HttpRequestMessage(HttpMethod.Post, TokenGenerationUrl))
            {
                // Add the Direct Line secret and userId to the token request. Note that in this sample code,
                // the Direct Line Secret required to generate the token is stored in appsettings.json
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["DirectLineSecret"]);
                request.Content = new StringContent(JsonConvert.SerializeObject(new { User = new { Id = userId } }), Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        // Generate the token and return it to the calling function
                        if (response.IsSuccessStatusCode)
                        {
                            var body = await response.Content.ReadAsStringAsync();
                            var dlToken = JsonConvert.DeserializeObject<DirectLineToken>(body);
                            dlToken.userId = userId;
                            return dlToken;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Calls to generate a token will return json in the format of this class.
        /// </summary>
        public class DirectLineToken
        {
            public string userId { get; set; }
            public string conversationId { get; set; }
            public string token { get; set; }
            public int expires_in { get; set; }
            public string streamUrl { get; set; }
        }
    }
}
