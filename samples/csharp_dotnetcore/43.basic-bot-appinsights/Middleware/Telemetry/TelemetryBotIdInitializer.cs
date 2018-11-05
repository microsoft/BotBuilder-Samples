using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BasicBot.Middleware.Telemetry
{
    /// <summary>
    /// Initializer that sets the user ID based on Bot data.
    /// </summary>
    public class TelemetryBotIdInitializer : ITelemetryInitializer
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TelemetryBotIdInitializer(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Initialize(ITelemetry telemetry)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && (telemetry is RequestTelemetry || telemetry is EventTelemetry))
            {
                // can't read from the request body at this point, as the
                // request stream has already been disposed.
                JObject body = _memoryCache.Get(context.TraceIdentifier) as JObject;
                if (body != null)
                {
                    var userId = (string)body["from"]?["id"];
                    var channelId = (string)body["channelId"];
                    var conversationId = (string)body["conversation"]?["id"];

                    // Set the user id on the Application Insights telemetry item.
                    telemetry.Context.User.Id = channelId + userId;

                    // Set the session id on the Application Insights telemetry item.
                    telemetry.Context.Session.Id = conversationId;
                }
            }
        }
    }
}
