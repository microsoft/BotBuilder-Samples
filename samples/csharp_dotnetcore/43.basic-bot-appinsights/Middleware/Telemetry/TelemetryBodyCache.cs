using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BasicBot.Middleware.Telemetry
{
    /// <summary>
    /// Application Insights does not enable getting the body of the HTTP request.
    /// This hooks into ASP.Net middleware to enable saving.
    /// This is registered as a Singleton.
    /// </summary>
    public class TelemetryBodyCache
    {
        private readonly ConcurrentDictionary<string, JObject> _bodyByTraceInitializer;

        public TelemetryBodyCache()
        {
            _bodyByTraceInitializer = new ConcurrentDictionary<string, JObject>();
        }

        public JObject Get(string traceInitializer)
        {
            if (_bodyByTraceInitializer.TryGetValue(traceInitializer, out var value))
            {
                return value;
            }

            return null;
        }

        public bool Set(string traceInitializer, string body)
        {
            try
            {
                var activity = JObject.Parse(body);
                _bodyByTraceInitializer[traceInitializer] = activity;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
