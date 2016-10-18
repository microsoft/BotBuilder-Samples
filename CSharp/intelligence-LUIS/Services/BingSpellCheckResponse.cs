namespace LuisBot.Services
{
    public class BingSpellCheckResponse
    {
        public string _type { get; set; }

        public Flaggedtoken[] flaggedTokens { get; set; }

        public Error error { get; set; }
    }

    public class Flaggedtoken
    {
        public int offset { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public Suggestion[] suggestions { get; set; }
    }

    public class Suggestion
    {
        public string suggestion { get; set; }
        public int score { get; set; }
    }

    public class Error
    {
        public int statusCode { get; set; }
        public string message { get; set; }
    }
}