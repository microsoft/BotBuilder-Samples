using System;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Bot.Builder.Dialogs;

namespace BasicBot.Middleware.Telemetry
{
    public static class TelemetryTrackWaterfallStepExtension
    {
        /// <summary>
        /// Tracks single Waterfall step in Application Insights.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="telemetryClient">The <seealso cref="TelemetryClient"/>.</param>
        /// <param name="waterfallStepContext">Encapsulates a method that has no parameters and returns a value of the type specified by the TResult parameter.</param>
        public static void TrackWaterfallStep(this TelemetryClient telemetryClient, WaterfallStepContext waterfallStepContext, string stepFriendlyName)
        {
            if (waterfallStepContext == null)
            {
                throw new ArgumentNullException(nameof(waterfallStepContext));
            }

            if (string.IsNullOrWhiteSpace(stepFriendlyName))
            {
                throw new ArgumentNullException(stepFriendlyName);
            }

            var eventName = waterfallStepContext.Stack.Last().Id + stepFriendlyName;

            EventTelemetry evt = new EventTelemetry(eventName);

            // Log the event into Application Insights
            telemetryClient.TrackEvent(evt);
        }
    }
}
