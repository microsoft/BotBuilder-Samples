namespace SpeechToText.Services
{
    public class AccessTokenInfo
    {
        public string access_token { get; set; }

        public string token_type { get; set; }

        public int expires_in { get; set; }

        public string scope { get; set; }
    }
}