// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ImmediateAcceptBot.BackgroundQueue;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImmediateAcceptBot
{
    /// <summary>
    /// The <see cref="ImmediateAcceptAdapter"/>will authenticate the incoming request, and queue to be 
    /// processed by the configured background service if possible.
    /// </summary>
    /// <remarks>
    /// If the activity is Not an Invoke, and DeliveryMode is Not ExpectReplies, and this
    /// is NOT a Get request to upgrade to WebSockets, then the activity will be enqueuedto be processed
    /// on a background thread.
    /// </remarks>
    public class ImmediateAcceptAdapter : BotFrameworkHttpAdapter, IBotFrameworkHttpAdapter
    {
        private readonly IActivityTaskQueue _activityTaskQueue;

        /// <summary>
        /// Create an instance of <see cref="ImmediateAcceptAdapter"/>.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="activityTaskQueue"></param>
        public ImmediateAcceptAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, IActivityTaskQueue activityTaskQueue)
            : base(configuration, logger)
        {
            _activityTaskQueue = activityTaskQueue;

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await turnContext.SendActivityAsync("The bot encountered an error or bug.");
                await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }

        /// <summary>
        /// This method can be called from inside a POST method on any Controller implementation.  If the activity is Not an Invoke, and
        /// DeliveryMode is Not ExpectReplies, and this is NOT a Get request to upgrade to WebSockets, then the activity will be enqueued
        /// to be processed on a background thread.
        /// </summary>
        /// <remarks>
        /// Note, this is an ImmediateAccept and BackgroundProcessing override of: 
        /// Task IBotFrameworkHttpAdapter.ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default);
        /// </remarks>
        /// <param name="httpRequest">The HTTP request object, typically in a POST handler by a Controller.</param>
        /// <param name="httpResponse">The HTTP response object.</param>
        /// <param name="bot">The bot implementation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        async Task IBotFrameworkHttpAdapter.ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            // Get is a socket exchange request, so should be processed by base BotFrameworkHttpAdapter
            if (httpRequest.Method == HttpMethods.Get)
            {
                await base.ProcessAsync(httpRequest, httpResponse, bot, cancellationToken);
            }
            else
            {
                // Deserialize the incoming Activity
                var activity = await HttpHelper.ReadRequestAsync<Activity>(httpRequest).ConfigureAwait(false);

                if (string.IsNullOrEmpty(activity?.Type))
                {
                    httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (activity.Type == ActivityTypes.Invoke || activity.DeliveryMode == DeliveryModes.ExpectReplies)
                {
                    // NOTE: Invoke and ExpectReplies cannot be performed async, the response must be written before the calling thread is released.
                    await base.ProcessAsync(httpRequest, httpResponse, bot, cancellationToken);
                }
                else
                {
                    // Grab the auth header from the inbound http request
                    var authHeader = httpRequest.Headers["Authorization"];

                    try
                    {
                        // If authentication passes, queue a work item to process the inbound activity with the bot
                        var claimsIdentity = await JwtTokenValidation.AuthenticateRequest(activity, authHeader, CredentialProvider, ChannelProvider, HttpClient).ConfigureAwait(false);

                        // Queue the activity to be processed by the ActivityBackgroundService
                        _activityTaskQueue.QueueBackgroundActivity(claimsIdentity, activity);

                        // Activity has been queued to process, so return Ok immediately
                        httpResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // handle unauthorized here as this layer creates the http response
                        httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                }
            }
        }
    }
}
