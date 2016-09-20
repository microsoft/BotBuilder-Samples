namespace LuisBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class HotelsQuery
    {
        [Prompt("Please enter your {&}")]
        [Optional]
        public string Destination { get; set; }

        [Prompt("Near which Airport")]
        [Optional]
        public string AirportCode { get; set; }
    }
}