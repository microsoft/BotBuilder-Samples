// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.PersonalityChat
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SupportBot.Models;

    /// <summary>
    /// Personality chat provider
    /// </summary>
    public class PersonalityChatProvider
    {
        /// <summary>
        /// Gets response from personality chat.
        /// </summary>
        /// <param name="query">query.</param>
        /// <returns>Personality chat response.</returns>
        internal async Task<PersonalityChatResponse> GetResponse(string query)
        {
            var request = new PersonalityChatRequest { Query = query, PersonaName = "Friendly", ResponseCount = 1, Metadata = "{\"RestrictedIntentsEnabled\" : \"false\"} , {\"CheckChat\" : \"true\"}, {\"CheckAdult\" : \"true\"}" };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ocp-apim-subscription-key", Constants.PersonalityChatKey);
                var result = await client.PostAsync("https://smarttalk.azure-api.net/pc/api/v1/personalitychat", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<PersonalityChatResponse>(resultContent);
                return response;
            }
        }
    }
}
