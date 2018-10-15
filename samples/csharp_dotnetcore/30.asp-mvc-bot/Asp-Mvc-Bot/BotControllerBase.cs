// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Asp_Mvc_Bot
{
    /// <summary>
    /// A base class for Controllers that act as bots. To use this add a Controller to the Visual Studio project
    /// as usual and then just edit it to swap out the base Controller class for this class.
    /// </summary>
    public abstract class BotControllerBase : Controller
    {
        public static readonly JsonSerializer BotMessageSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ReadOnlyJsonContractResolver(),
            Converters = new List<JsonConverter> { new Iso8601TimeSpanConverter() },
        });

        /// <summary>
        /// Initializes a new instance of the <see cref="BotControllerBase"/> class.
        /// </summary>
        /// <param name="options">The <see cref="BotFrameworkOptions"/> to use.</param>
        protected BotControllerBase(BotFrameworkOptions options = null)
        {
            Options = options ?? new BotFrameworkOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BotControllerBase"/> class.
        /// This create the BotFrameworkOptions needed by the BotFrameworkAdapter from the BotConfiguration.
        /// </summary>
        /// <param name="botConfig">A <see cref="BotConfiguration"/> object.</param>.
        /// <param name="name">The name of the specific section of confuguration that shoudl be read.</param>
        protected BotControllerBase(BotConfiguration botConfig, string name)
        {
            // Retrieve current endpoint.
            var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == name).FirstOrDefault();
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{name}'.");
            }

            Options = new BotFrameworkOptions
            {
                CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword),
                OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong in bot1.");
                },
            };
        }

        /// <summary>
        /// Gets a <see cref="BotFrameworkOptions"/> object.
        /// </summary>
        /// <value>
        /// The <see cref="BotFrameworkOptions"/> object used to create the adapter.
        /// </value>
        protected BotFrameworkOptions Options { get; }

        [HttpPost]
        public async Task PostAsync()
        {
            var activity = default(Activity);

            using (var bodyReader = new JsonTextReader(new StreamReader(Request.Body, Encoding.UTF8)))
            {
                activity = BotMessageSerializer.Deserialize<Activity>(bodyReader);
            }

            var botFrameworkAdapter = new BotFrameworkAdapter(Options.CredentialProvider);

            var invokeResponse = await botFrameworkAdapter.ProcessActivityAsync(
                Request.Headers["Authorization"],
                activity,
                OnTurnAsync,
                default(CancellationToken));

            if (invokeResponse == null)
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Response.ContentType = "application/json";
                Response.StatusCode = invokeResponse.Status;

                using (var writer = new StreamWriter(Response.Body))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        BotMessageSerializer.Serialize(jsonWriter, invokeResponse.Body);
                    }
                }
            }
        }

        /// <summary>
        /// Override OnTurnAsync to implement teh bot specific logic. This is exactly the same as implementing
        /// the IBot OnTurnAsync except here we can have multiple independent bots in the same project in addition
        /// to including arbitray other MVC Controllers.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        protected abstract Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken);
    }
}
