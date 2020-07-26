# Immediate Accept Bot

Built starting from Bot Framework v4 echo bot sample.

This example demonstrates how to create a simple bot that accepts input from the user and echoes it back.  All incoming activities are processed on a background thread, alleviating 15 second timeout concerns.

ImmediateAcceptAdapter verifies the authorization header, adds the message to a Microsoft.Extensions.Hosting.BackgroundService (HostedActivityService) for processing, and writes HttpStatusCode.OK to HttpResponse.  This causes all messages sent by the bot to effectively be proactive.

## ImmediateAcceptAdapter.cs

New method in BotFrameworkHttpAdapter implementation:

```cs
/// <summary>
/// This method can be called from inside a POST method on any Controller implementation.  If the activity is Not an Invoke, and
/// DeliveryMode is Not ExpectReplies, and this is not a Get request to upgrade to WebSockets, then the activity will be enqueued
/// to be processed on a background thread.
/// 
/// 
/// 
/// Note, this is an ImmediateAccept and BackgroundProcessing replacement for: 
/// Task ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default);
/// </summary>
/// <param name="httpRequest">The HTTP request object, typically in a POST handler by a Controller.</param>
/// <param name="httpResponse">The HTTP response object.</param>
/// <param name="bot">The bot implementation.</param>
/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive
///     notice of cancellation.</param>
/// <returns>A task that represents the work queued to execute.</returns>
public async Task ProcessOnBackgroundThreadAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default)
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
        await ProcessAsync(httpRequest, httpResponse, bot, cancellationToken);
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
            // NOTE: Invoke and ExpectReplies cannot be performed async,
            // the response must be written before the calling thread is released.
            await ProcessAsync(httpRequest, httpResponse, bot, cancellationToken);
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
```

## Startup.cs

Register BackgroundServices and classes:

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers().AddNewtonsoftJson();

    // Activity specific BackgroundService for processing athenticated activities.
    services.AddHostedService<HostedActivityService>();
    // Generic BackgroundService for processing tasks.
    services.AddHostedService<HostedTaskService>();
            
    // BackgroundTaskQueue and ActivityTaskQueue are the entry points for
    // the enqueueing activities or tasks to be processed by the BackgroundService.
    services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
    services.AddSingleton<IActivityTaskQueue, ActivityTaskQueue>();

    // Configure the ShutdownTimeout based on appsettings.
    services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(Configuration.GetValue<int>("ShutdownTimeoutSeconds")));

    // Create the Bot Framework Adapter with error handling enabled.
    services.AddSingleton<ImmediateAcceptAdapter>();

    // Create the bot. In this case the ASP Controller and ImmediateAcceptAdapter is expecting an IBot.
    services.AddSingleton<IBot, EchoBot>();
}
```

## Interacting with the Bot

send: 4 seconds   ...  and the bot will pause for 4 seconds while processing your message.
send: 4 background   ...  and the bot will push your message to an additional background thread to process for 4 seconds.

## Additional Resources

QueuedHostedService is from https://github.com/dotnet/AspNetCore.Docs/tree/master/aspnetcore/fundamentals/host/hosted-services/samples/3.x/BackgroundTasksSample
