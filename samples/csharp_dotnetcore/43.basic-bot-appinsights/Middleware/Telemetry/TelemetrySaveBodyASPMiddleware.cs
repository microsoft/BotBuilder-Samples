using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace BasicBot.Middleware.Telemetry
{
    public class TelemetrySaveBodyASPMiddleware
    {
        private readonly RequestDelegate _next;

        public TelemetrySaveBodyASPMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            IMemoryCache memoryCache = context.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

            if (memoryCache != null)
            {
                context.Request.EnableBuffering();

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(3)); // Keep in cache for this time, reset time if accessed.

                    // Save data in cache.
                    memoryCache.Set(context.TraceIdentifier, JObject.Parse(reader.ReadToEnd()), cacheEntryOptions);
                }

                context.Request.Body.Position = 0;
            }

            await _next(context);
        }
    }
}
