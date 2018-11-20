using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace BasicBot.Middleware.Telemetry
{
    public static class TelemetryBotDependencyExtension
    {
        public const string DependencyType = "Bot";

        /// <summary>
        /// Send information about an dependency in the bot application.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="telemetryClient">The <seealso cref="TelemetryClient"/>.</param>
        /// <param name="function">Encapsulates a method that has no parameters and returns a value of the type specified by the TResult parameter.</param>
        /// <param name="dependencyName">Name of the command initiated with this dependency call. Low cardinality value. Examples are stored procedure name and URL path template.</param>
        /// <param name="dependencyData">Command initiated by this dependency call. For example, Middleware.</param>
        /// <returns>The return value of the method that this delegate encapsulates.</returns>
        /// <remarks>The action delegate will be timed and a Application Insights dependency record will be created.</remarks>
        public static TResult TrackBotDependency<TResult>(this TelemetryClient telemetryClient, Func<TResult> function, string dependencyName, string dependencyData = null)
        {
            if (dependencyName == null)
            {
                throw new ArgumentNullException(nameof(dependencyName));
            }

            var timer = new Stopwatch();
            var startTime = DateTimeOffset.Now;
            var success = true;

            timer.Start();

            try
            {
                return function();
            }
            catch (Exception)
            {
                success = false;
                throw;
            }
            finally
            {
                timer.Stop();

                // Log the dependency into Application Insights
                telemetryClient.TrackDependency(DependencyType, dependencyName, dependencyData, startTime, timer.Elapsed, success);
            }
        }
    }
}
