// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;

namespace Microsoft.BotBuilderSamples.DialogSkillBot.CognitiveModels
{
    // Extends the partial FlightBooking class with methods and properties that simplify accessing entities in the LUIS results.
    public partial class FlightBooking
    {
        public (string From, string Airport) FromEntities
        {
            get
            {
                var fromValue = Entities?._instance?.From?.FirstOrDefault()?.Text;
                var fromAirportValue = Entities?.From?.FirstOrDefault()?.Airport?.FirstOrDefault()?.FirstOrDefault();
                return (fromValue, fromAirportValue);
            }
        }

        public (string To, string Airport) ToEntities
        {
            get
            {
                var toValue = Entities?._instance?.To?.FirstOrDefault()?.Text;
                var toAirportValue = Entities?.To?.FirstOrDefault()?.Airport?.FirstOrDefault()?.FirstOrDefault();
                return (toValue, toAirportValue);
            }
        }

        // This value will be a TIMEX. We are only interested in the Date part, so grab the first result and drop the Time part.
        // TIMEX is a format that represents DateTime expressions that include some ambiguity, such as a missing Year.
        public string TravelDate
            => Enumerable.FirstOrDefault<string>(Entities.datetime?.FirstOrDefault()?.Expressions)?.Split('T')[0];
    }
}
