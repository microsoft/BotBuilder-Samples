// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.BotBuilderSamples;

namespace CoreBot.CognitiveModels
{
    // extension methods that simplify accessing entities in FlightBooking results from luis
    public static class FlightBookingEx
    {
        // Gets the value of the From Entity and From Airport if present
        // In some cases LUIS will recognize the From entity as a valid city, but if the CompositeEntity
        // is not trained it will not recognize the city as a valid Airport.
        public static (string From, string Airport) GetFromEntities(this FlightBooking luisResult)
        {
            var fromValue = luisResult.Entities?._instance?.From?.FirstOrDefault()?.Text;
            var fromAirportValue = luisResult.Entities?.From?.FirstOrDefault()?.Airport?.FirstOrDefault()?.FirstOrDefault();
            return (fromValue, fromAirportValue);
        }

        public static (string To, string Airport) GetToEntities(this FlightBooking luisResult)
        {
            var toValue = luisResult.Entities?._instance?.To?.FirstOrDefault()?.Text;
            var toAirportValue = luisResult.Entities?.To?.FirstOrDefault()?.Airport?.FirstOrDefault()?.FirstOrDefault();
            return (toValue, toAirportValue);
        }

        public static string GetTravelDate(this FlightBooking luisResult)
        {
            return luisResult.Entities.datetime?.FirstOrDefault()?.Expressions.FirstOrDefault();
        }
    }
}
