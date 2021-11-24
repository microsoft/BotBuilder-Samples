using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.Bot.Builder.AI.CLU.Tests
{
    public static class TestUtilities
    {
        public static TurnContext BuildTurnContextForUtterance(string utterance)
        {
            var testAdapter = new TestAdapter();
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                Text = utterance,
                Conversation = new ConversationAccount(),
                Recipient = new ChannelAccount(),
                From = new ChannelAccount(),
            };
            return new TurnContext(testAdapter, activity);
        }

        public static void AssertCluEntities(JObject entitiesObject, string fromCity, string toCity, string dateText)
        {
            var cluEntityList = JsonConvert.DeserializeObject<CluEntity[]>(entitiesObject["entities"].ToString());
            CluEntity[] fromCityList = cluEntityList.Where(e => e.category == "fromCity").ToArray();
            CluEntity[] toCityList = cluEntityList.Where(e => e.category == "toCity").ToArray();
            CluEntity[] flightDateList = cluEntityList.Where(e => e.category == "flightDate").ToArray();

            Assert.Contains(fromCityList, entity => entity.text == fromCity);
            Assert.Contains(toCityList, entity => entity.text == toCity);
            Assert.Contains(flightDateList, entity => entity.text == dateText);
        }
    }
}
