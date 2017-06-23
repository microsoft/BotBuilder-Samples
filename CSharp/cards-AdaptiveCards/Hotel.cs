namespace BotBuilder.Samples.AdaptiveCards
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Hotel
    {
        public string Name { get; set; }

        public int Rating { get; set; }

        public int NumberOfReviews { get; set; }

        public int PriceStarting { get; set; }

        public string Image { get;  set; }

        public IEnumerable<string> MoreImages { get; set; }

        public string Location { get;  set; }
    }
}