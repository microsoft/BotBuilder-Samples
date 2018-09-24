// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class ReservationProperty
    {
        /**
         * Reservation property class. 
         *   This is a self contained class that exposes a bunch of public methods to 
         *     evaluate if we have a complete instance (all required properties filled in)
         *     generate reply text 
         *       based on missing properties
         *       with all information that"s already been captured
         *       to confirm reservation
         *       to provide contextual help
         *   Also exposes two static methods to construct a reservations object based on 
         *     LUIS results object
         *     arbitrary object        
         *   
         */
        // Possible LUIS entities. You can refer to dialogs\dispatcher\resources\entities.lu for list of entities
        public static string[] LuisEntities = { "confirmationList", "number", "datetime", "cafeLocation", "userName_patternAny", "userName" };

        // Consts for LUIS entities.
        private static string partySizeEntity = LuisEntities[1];
        private static string dateTimeEntity = LuisEntities[2];
        private static string LocationEntity = LuisEntities[3];
        private static string ConfirmationEntity = "confirmationList";
        private static int FourWeeks = 4;
        private const int MaxPartySize = 10;

        // Date constraints for reservations
        private static string[] ReservationDateConstraints = {        /* Date for reservations must be        */
            TimexCreator.ThisWeek(),                       /* - a date in this week .OR.           */
            TimexCreator.NextWeeksFromToday(FourWeeks),  /* - a date in the next 4 weeks .AND.   */
        };

        // Time constraints for reservations
        private static string[] ReservationTimeConstraints = {        /* Time for reservations must be   */
            TimexCreator.Daytime,                         /* - daytime or                    */
        };

        public string Id { get; }

        public string Date { get; set; }

        public string DateLGString { get; set; }

        public string Time { get; set; }

        public string TimeLGString { get; set; }

        public string DateTimeLGString { get; set; }

        public int PartySize { get; set; }

        public string Location { get; set; }

        public bool ReservationConfirmed { get; set; }

        public bool NeedsChange { get; set; }

        public object MetaData { get; set; }

        /**
        * Reservation Property constructor.
        * 
        * @param {String} id reservation id
        * @param {String} date reservation date
        * @param {String} time reservation time
        * @param {Number} partySize number of guests in reservation
        * @param {String} location reservation location
        */
        public ReservationProperty(string id = null,
            string date = null,
            string time = null,
            int partySize = 0,
            string location = null,
            bool reservationConfirmed = false,
            bool needsChange = false,
            object metaData = null)
        {
            Id = id ?? Guid.NewGuid().ToString();
            Date = date ?? string.Empty;
            Time = time ?? string.Empty;
            DateLGString = string.Empty;
            TimeLGString = string.Empty;
            DateTimeLGString = string.Empty;
            PartySize = partySize;
            Location = location ?? string.Empty;
            ReservationConfirmed = reservationConfirmed;
            NeedsChange = needsChange;
            MetaData = metaData;
        }

        /**
         * Helper method to evalute if we have all required properties filled.
         * 
         * @returns {Boolean} true if we have a complete reservation property
         */
        public bool HaveCompleteReservation()
        {
            return !string.IsNullOrWhiteSpace(Id) &&
                   !string.IsNullOrWhiteSpace(Date) &&
                   !string.IsNullOrWhiteSpace(Time) &&
                   PartySize > 0 &&
                   !string.IsNullOrWhiteSpace(Location);
        }

        /**
         * Helper method to update Reservation property with information passed in via the onTurnProperty object
         * 
         * @param {OnTurnProperty}  
         * @returns {ReservationResult}  
         */
        public ReservationResult UpdateProperties(OnTurnProperty onTurnProperty)
        {
            var returnResult = new ReservationResult(this);
            return Validate(onTurnProperty, returnResult);
        }


        /**
         * Helper method for Language Generation read out based on current reservation property object
         * 
         * @returns {String}
         */
        public string GetMissingPropertyReadOut()
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                return "What city?";
            }
            else if (string.IsNullOrWhiteSpace(Date))
            {
                return "When do you want to come in?";
            }
            else if (string.IsNullOrWhiteSpace(Time))
            {
                return "What time?";
            }
            else if (PartySize == 0)
            {
                return "How many guests ?";
            }

            return string.Empty;
        }

        /// <summary>
        /// Helper method for Language Generation read out based on properties that have been captured.
        /// </summary>
        /// <returns>A <see cref="string"/> which provides a sentence.</returns>
        public string GetGroundedPropertiesReadOut()
        {
            var today = DateTime.Now;
            if (HaveCompleteReservation())
            {
                return ConfirmationReadOut();
            }

            var groundedProperties = string.Empty;
            if (PartySize > 0)
            {
                groundedProperties += $" for {PartySize} guests,";
                if (!string.IsNullOrWhiteSpace(Location))
                {
                    groundedProperties += $" in our {Location} store,";
                    if (!string.IsNullOrWhiteSpace(Date) && !string.IsNullOrWhiteSpace(Time))
                    {
                        groundedProperties += " for " + new TimexProperty(Date + "T" + Time).ToNaturalLanguage(today) + ".";
                    }
                    else if (!string.IsNullOrWhiteSpace(Date))
                    {
                        groundedProperties += " for " + new TimexProperty(Date).ToNaturalLanguage(today);
                    }
                    else if (!string.IsNullOrWhiteSpace(TimeLGString))
                    {
                        groundedProperties += $" for {TimeLGString}";
                    }

                    if (string.IsNullOrWhiteSpace(groundedProperties))
                    {
                        return groundedProperties;
                    }

                    return $"Ok. I have a table {groundedProperties}";
                }
            }

            return string.Empty;
        }

        /**
         * Helper to generate confirmation read out string.
         * 
         * @returns {String}
         */
        public string ConfirmationReadOut()
        {
            var today = DateTime.Now;
            return PartySize + " at our " + Location + " store for " + new TimexProperty(Date + "T" + Time).ToNaturalLanguage(today) + ".";
        }

        /**
         * Helper to generate help read out string.
         * 
         * @returns {String}
         */
        public string HelpReadOut()
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                return "We have cafe locations in Seattle, Bellevue, Redmond and Renton.";
            }
            else if (string.IsNullOrWhiteSpace(Date))
            {
                return "I can help you reserve a table up to 4 weeks from today..You can say \"tomorrow\", \"next sunday at 3pm\"...";
            }
            else if (string.IsNullOrWhiteSpace(Time))
            {
                return "All our cafe locations are open 6AM - 6PM.";
            }
            else if (PartySize == 0)
            {
                return "I can help you book a table for up to 10 guests..";
            }

            return string.Empty;
        }

        /**
         * Static method to create a new instance of Reservation property based on onTurnProperty object
         * 
         * @param {OnTurnProperty} 
         * @returns {ReservationResult} 
         */
        public static ReservationResult FromOnTurnProperty(OnTurnProperty onTurnProperty)
        {
            var returnResult = new ReservationResult(new ReservationProperty());
            return Validate(onTurnProperty, returnResult);
        }

        /**
         * Helper function to validate input and return results based on validation constraints
         * 
         * @param {Object} onTurnProperty 
         * @param {ReservationResult} return result object 
         */
        public static ReservationResult Validate(OnTurnProperty onTurnProperty, ReservationResult returnResult)
        {
            if (onTurnProperty == null || onTurnProperty.Entities.Count == 0)
            {
                return returnResult;
            }

            // We only will pull number -> party size, datetimeV2 -> date and time, cafeLocation -> location.
            var numberEntity = onTurnProperty.Entities.Find(item => item.EntityName.Equals(partySizeEntity));
            var dateTimeEntity = onTurnProperty.Entities.Find(item => item.EntityName.Equals(ReservationProperty.dateTimeEntity));
            var locationEntity = onTurnProperty.Entities.Find(item => item.EntityName.Equals(LocationEntity));
            var confirmationEntity = onTurnProperty.Entities.Find(item => item.EntityName.Equals(ConfirmationEntity));

            if (numberEntity != null)
            {
                // We only accept MaxPartySize in a reservation.
                if (int.Parse(numberEntity.Value as string) > MaxPartySize)
                {
                    returnResult.Outcome.Add(new ReservationOutcome("Sorry. " + int.Parse(numberEntity.Value as string) + " does not work. I can only accept up to 10 guests in a reservation.", partySizeEntity));
                    returnResult.Status = ReservationStatus.Incomplete;
                }
                else
                {
                    returnResult.NewReservation.PartySize = (int)numberEntity.Value;
                }
            }

            if (dateTimeEntity != null)
            {
                // Get parsed date time from TIMEX
                // LUIS returns a timex expression and so get and un-wrap that.
                // Take the first date time since book table scenario does not have to deal with multiple date times or date time ranges.
                var timeProp = dateTimeEntity.Value as string;
                if (timeProp != null)
                {
                    var today = DateTime.Now;
                    var parsedTimex = new TimexProperty(timeProp);

                    // see if the date meets our constraints
                    if (parsedTimex.DayOfMonth != null && parsedTimex.Year != null && parsedTimex.Month != null)
                    {
                        var lDate = DateTimeOffset.Parse($"{parsedTimex.Year}-${parsedTimex.Month}-${parsedTimex.DayOfMonth}");
                        returnResult.NewReservation.Date = lDate.UtcDateTime.ToString("o").Split("T")[0];
                        returnResult.NewReservation.DateLGString = new TimexProperty(returnResult.NewReservation.Date).ToNaturalLanguage(today);
                        var validDate = TimexRangeResolver.Evaluate(dateTimeEntity.Value as string[], ReservationDateConstraints);
                        if (validDate != null || (validDate.Count == 0))
                        {
                            // Validation failed!
                            returnResult.Outcome.Add(new ReservationOutcome(
                                $"Sorry. {returnResult.NewReservation.DateLGString} does not work.  "
                                + "I can only make reservations for the next 4 weeks.", ReservationProperty.dateTimeEntity));
                            returnResult.NewReservation.Date = string.Empty;
                            returnResult.Status = ReservationStatus.Incomplete;
                        }
                    }

                    // see if the time meets our constraints
                    if (parsedTimex.Hour != null &&
                        parsedTimex.Minute != null &&
                        parsedTimex.Second != null)
                    {
                        var validtime = TimexRangeResolver.Evaluate(dateTimeEntity.Value as string[], ReservationTimeConstraints);

                        returnResult.NewReservation.Time = ((int)parsedTimex.Hour).ToString("D2");
                        returnResult.NewReservation.Time += ":";
                        returnResult.NewReservation.Time += ((int)parsedTimex.Minute).ToString("D2");
                        returnResult.NewReservation.Time += ":";
                        returnResult.NewReservation.Time += ((int)parsedTimex.Second).ToString("D2");

                        if (validtime != null || (validtime.Count == 0))
                        {
                            // Validation failed!
                            returnResult.Outcome.Add(new ReservationOutcome("Sorry, that time does not work. I can only make reservations that are in the daytime (6AM - 6PM)", ReservationProperty.dateTimeEntity));
                            returnResult.NewReservation.Time = string.Empty;
                            returnResult.Status = ReservationStatus.Incomplete;
                        }
                    }

                    // Get date time LG string if we have both date and time.
                    if (string.IsNullOrWhiteSpace(returnResult.NewReservation.Date) && string.IsNullOrWhiteSpace(returnResult.NewReservation.Time))
                    {
                        returnResult.NewReservation.DateTimeLGString = new TimexProperty(returnResult.NewReservation.Date + "T" + returnResult.NewReservation.Time).ToNaturalLanguage(today);
                    }
                }
            }

            // Take the first found value.
            if (locationEntity != null)
            {
                var cafeLocation = ((JObject)locationEntity.Value)[0][0];

                // Capitalize cafe location.
                returnResult.NewReservation.Location = char.ToUpper(((string)cafeLocation)[0]) + ((string)cafeLocation).Substring(1);
            }

            // Accept confirmation entity if available only if we have a complete reservation
            if (confirmationEntity != null)
            {
                if ((string)((JObject)confirmationEntity.Value)[0][0] == "yes")
                {
                    returnResult.NewReservation.ReservationConfirmed = true;
                    returnResult.NewReservation.NeedsChange = false;
                }
                else
                {
                    returnResult.NewReservation.NeedsChange = true;
                    returnResult.NewReservation.ReservationConfirmed = false;
                }
            }

            return returnResult;
        }
    }
}
