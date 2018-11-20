using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
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
            if (context.Request.Method == "POST")
            {
                IMemoryCache memoryCache = context.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

                if (memoryCache != null)
                {
                    context.Request.EnableBuffering();
                    try
                    {
                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                        {
                            // Set cache options.
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(3)); // Keep in cache for this time, reset time if accessed.

                            var body = reader.ReadToEnd();
                            var jsonObject = JObject.Parse(body);

                            // Save data in cache.
                            memoryCache.Set(context.TraceIdentifier, jsonObject, cacheEntryOptions);
                        }
                    }
                    catch (JsonReaderException)
                    {
                        // Request not json.
                    }
                    finally
                    {
                        // rewind for next middleware.
                        context.Request.Body.Position = 0;
                    }
                }
            }

            await _next(context);
        }
    }
}
