namespace SpeechToText.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Configuration;
    using Newtonsoft.Json;

    public sealed class Authentication
    {
        private static readonly object LockObject;
        private static readonly string ApiKey;
        private AccessTokenInfo token;
        private Timer timer;

        static Authentication()
        {
            LockObject = new object();
            ApiKey = WebConfigurationManager.AppSettings["MicrosoftSpeechApiKey"];
        }

        private Authentication()
        {
        }

        public static Authentication Instance { get; } = new Authentication();

        /// <summary>
        /// Gets the current access token.
        /// </summary>
        /// <returns>Current access token</returns>
        public AccessTokenInfo GetAccessToken()
        {
            // Token will be null first time the function is called.
            if (this.token == null)
            {
                lock (LockObject)
                {
                    // This condition will be true only once in the lifetime of the application
                    if (this.token == null)
                    {
                        this.RefreshToken();
                    }
                }
            }

            return this.token;
        }

        /// <summary>
        /// Issues a new AccessToken from the Speech Api
        /// </summary>
        /// This method couldn't be async because we are calling it inside of a lock.
        /// <returns>AccessToken</returns>
        private static AccessTokenInfo GetNewToken()
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", ApiKey },
                    { "client_secret", ApiKey },
                    { "scope", "https://speech.platform.bing.com" }
                };

                var content = new FormUrlEncodedContent(values);

                var response = client.PostAsync("https://oxford-speech.cloudapp.net/token/issueToken", content).Result;

                var responseString = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<AccessTokenInfo>(responseString);
            }
        }

        /// <summary>
        /// Refreshes the current token before it expires. This method will refresh the current access token.
        /// It will also schedule itself to run again before the newly acquired token's expiry by one minute.
        /// </summary>
        private void RefreshToken()
        {
            this.token = GetNewToken();
            this.timer?.Dispose();
            this.timer = new Timer(
                x => this.RefreshToken(), 
                null, 
                TimeSpan.FromSeconds(this.token.expires_in).Subtract(TimeSpan.FromMinutes(1)), // Specifies the delay before RefreshToken is invoked.
                TimeSpan.FromMilliseconds(-1)); // Indicates that this function will only run once
        }
    }
}