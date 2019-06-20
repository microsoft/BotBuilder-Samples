// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;

namespace Microsoft.BotBuilderSamples
{
    // Extends the partial FlightBooking class with methods and properties that simplify accessing entities in the luis results
    public partial class FlightBooking
    {
        // Gets the value of the From Entity and From Airport if present
        // In some cases LUIS will recognize the From entity as a valid city, but if the CompositeEntity
        // is not trained it will not recognize the city as a valid Airport.
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

        // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
        // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
        public string TravelDate
            => Entities.datetime?.FirstOrDefault()?.Expressions.FirstOrDefault()?.Split('T')[0];
    }
}
