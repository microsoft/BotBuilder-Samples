// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A helper class for extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Make an Http request to get the users image.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> used to make the Http request.</param>
        /// <param name="accessToken"> The Bearer token issue to the user.</param>
        /// <param name="endpoint">The Url to which the Http request will be made.</param>
        /// <returns>A <see cref="PhotoResponse"/> which is image data for a user.</returns>
        public static async Task<PhotoResponse> GetStreamWithAuthAsync(this HttpClient client, string accessToken, string endpoint)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using (var response = await client.GetAsync(endpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    var photoResponse = new PhotoResponse
                    {
                        Bytes = bytes,
                        ContentType = response.Content.Headers.ContentType?.ToString(),
                    };
                    return photoResponse;
                }

                return null;
            }
        }
    }
}
