// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Streaming;
using Microsoft.Bot.Streaming.Transport.WebSockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Controllers
{
    [Route("v3/directline")]
    [ApiController]
    public class DirectlineController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        private readonly ILogger _logger;

        public DirectlineController(IBotFrameworkHttpAdapter adapter, IBot bot, ILogger<DirectlineController> logger)
        {
            _adapter = adapter;
            _bot = bot;
            _logger = logger;
        }

        [HttpPost("tokens/generate")]
        [Produces("application/json")]
        public async Task<object> TokenGenerate()
        {
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync().ConfigureAwait(false);
            TokenGenerationParameters tokenGenerationParameters = null;
            try
            {
                tokenGenerationParameters = JsonConvert.DeserializeObject<TokenGenerationParameters>(requestBody);
            }
            catch (JsonException)
            {
                _logger.LogError("The request body is not a valid JSON object");
                return BadRequest("The request body is not a valid JSON object");
            }

            if (tokenGenerationParameters?.user?.Id == null || tokenGenerationParameters?.user?.Name == null)
            {
                _logger.LogWarning("No user specified, using default user id and name");
                tokenGenerationParameters = new TokenGenerationParameters();
                tokenGenerationParameters.user = new TokenGenerationParameters.User
                {
                    Id = "default-user-id",
                    Name = "default-user-name"
                };
            }

            // We just encode the user id and name in the token for this sample
            // Please choose appropriate security mechanisms for production scenarios
            // The token generated here should be validated when connecting to this endpoint below
            var token = tokenGenerationParameters.user.ToString();
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            token = Convert.ToBase64String(tokenBytes);

            return new
            {
                conversationId = Guid.NewGuid().ToString(),
                token
            };
        }

        [HttpGet("conversations/connect")]
        public async Task ConnectToConversation()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("Upgrade to WebSocket is required")).ConfigureAwait(false);
                return;
            }

            var token = Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                HttpContext.Response.StatusCode = 400;
                await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("The query parameter 'token' is required")).ConfigureAwait(false);
                return;
            }

            var conversationId = Request.Query["conversationId"];
            if (string.IsNullOrEmpty(conversationId))
            {
                _logger.LogInformation("No conversationId specified when connecting, generating new conversation id");
                conversationId = Guid.NewGuid().ToString();
            }

            // Again, this is just a sample. In production scenarios, you should validate the token
            // based on how you generated it in the TokenGenerate method above
            var tokenBytes = Convert.FromBase64String(token);
            var tokenString = Encoding.UTF8.GetString(tokenBytes);
            var tokenParts = tokenString.Split(';');
            if (tokenParts.Length != 2)
            {
                HttpContext.Response.StatusCode = 400;
                await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("The token is invalid")).ConfigureAwait(false);
                return;
            }

            var user = new ChannelAccount(tokenParts[0], tokenParts[1]);

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            if (webSocket == null)
            {
                HttpContext.Response.StatusCode = 500;
                _logger.LogError("Failed to accept WebSocket connection");
                return;
            }
            else
            {
                // This may not necessarily be an AdapterWithErrorHandler, but we need to access the custom ProcessActivityAsync method
                if (_adapter is AdapterWithErrorHandler adapterWithErrorHandler)
                {
                    var directlineClientRequestHander = new DirectlineRequestHandler(adapterWithErrorHandler, _bot, user, conversationId);
                    var wbServer = new WebSocketServer(webSocket, directlineClientRequestHander);
                    directlineClientRequestHander.WebSocketServer = wbServer;
                    await wbServer.StartAsync();
                }
                else
                {
                    HttpContext.Response.StatusCode = 500;
                    return;
                }
            }
        }

        public class TokenGenerationParameters
        {
            public class User
            {
                [JsonProperty("id")]
                public string Id { get; set; }
                [JsonProperty("name")]
                public string Name { get; set; }

                public override string ToString()
                {
                    return Id + ";" + Name;
                }
            }

            [JsonProperty("user")]
            public User user { get; set; }
        }
    }

    public class DirectlineRequestHandler : RequestHandler
    {
        private readonly AdapterWithErrorHandler _adapter;
        private readonly IBot _bot;
        private ChannelAccount _user;
        private readonly string channelId = "webchat";
        private readonly string _conversationId;

        public WebSocketServer WebSocketServer { get; set; }

        public DirectlineRequestHandler(AdapterWithErrorHandler adapter, IBot bot, ChannelAccount user, string conversationId)
        {
            _adapter = adapter;
            _bot = bot;
            _user = user;
            _conversationId = conversationId;
        }

        public override Task<StreamingResponse> ProcessRequestAsync(ReceiveRequest request, ILogger<RequestHandler> logger, object context = null, CancellationToken cancellationToken = default)
        {
            if (request.Verb == "POST")
            {
                // It's either creating a new conversation or sending an activity
                // First check if it's creating a new conversation
                if (request.Path == "/v3/directline/conversations")
                {
                    return ProcessCreateConversationRequestAsync(request, logger, context, cancellationToken);
                }

                // Then check if it's sending an activity
                var postActivityTemplate = TemplateParser.Parse("/v3/directline/conversations/{conversationId}/activities");
                var postActivityMatcher = new TemplateMatcher(postActivityTemplate, new RouteValueDictionary());
                var routeValues = new RouteValueDictionary();
                if (postActivityMatcher.TryMatch(request.Path, routeValues))
                {
                    var conversationId = routeValues["conversationId"] as string;
                    return ProcessPostActivityRequestAsync(request, logger, context, conversationId, _user, cancellationToken);
                }
            }

            throw new NotImplementedException($"Request {request.Verb} {request.Path}");
        }

        private async Task<StreamingResponse> ProcessCreateConversationRequestAsync(ReceiveRequest request, ILogger<RequestHandler> logger, object context = null, CancellationToken cancellationToken = default)
        {
            var conversation = new
            {
                conversationId = _conversationId,
            };

            await SendConversationUpdateToBotAsync(request, conversation.conversationId, logger, cancellationToken);

            var streamResponse = StreamingResponse.OK(new StringContent(JsonConvert.SerializeObject(conversation, SerializationSettings.DefaultSerializationSettings)));

            return streamResponse;
        }
         
        private async Task<StreamingResponse> ProcessPostActivityRequestAsync(ReceiveRequest request, ILogger<RequestHandler> logger, object context, string conversationId, ChannelAccount user, CancellationToken cancellationToken)
        {
            var activity = await request.ReadBodyAsJsonAsync<Activity>().ConfigureAwait(false);
            activity.Conversation = new ConversationAccount { Id = conversationId };
            activity.Id = Guid.NewGuid().ToString();
            activity.Timestamp = DateTime.UtcNow;
            activity.Recipient = new ChannelAccount { Id = "bot", Name = "bot" };

            // Echo back the activity to client first, so the client knows the activity has been received
            await SendActivityToClient(request, activity, logger, cancellationToken);

            var invokeResponse = await SendActivityToBot(request, activity, logger, cancellationToken);
            object result = (object)invokeResponse ?? new { activity.Id };
            var streamResponse = StreamingResponse.OK(new StringContent(JsonConvert.SerializeObject(result, SerializationSettings.DefaultSerializationSettings)));
            return streamResponse;
        }

        private async Task<InvokeResponse> SendConversationUpdateToBotAsync(ReceiveRequest requestContext, string conversationId, ILogger logger, CancellationToken cancellationToken)
        {
            var update = new Activity
            {
                Type = ActivityTypes.ConversationUpdate,
                MembersAdded = new[] { _user },
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                ChannelId = channelId,
                Conversation = new ConversationAccount { Id = conversationId },
                Recipient = new ChannelAccount { Id = "bot", Name = "bot" },
                From = _user
            };
            return await SendActivityToBot(requestContext, update, logger, cancellationToken);
        }

        private async Task<InvokeResponse> SendActivityToBot(ReceiveRequest requestContext, Activity activity, ILogger logger, CancellationToken cancellationToken)
        {
            var authenticationRequestResult = new AuthenticateRequestResult()
            {
                // It might be helpful to fill more meaningful data here for logging 
                Audience = "https://api.botframework.com",
                ClaimsIdentity = new ClaimsIdentity(),
            };
            authenticationRequestResult.ConnectorFactory = new StreamingConnectionFactory(WebSocketServer, logger);
            return await _adapter.CustomProcessActivityAsync(authenticationRequestResult, activity, _bot.OnTurnAsync, cancellationToken);
        }

        private async Task<ReceiveResponse> SendActivityToClient(ReceiveRequest requestContext, Activity activity, ILogger logger, CancellationToken cancellationToken)
        {
            var clientRequest = new StreamingRequest
            {
                // Stream client is expecting the path to be relative to /v3/directline
                Path = requestContext.Path.Replace("/v3/directline", ""),
                Verb = requestContext.Verb
            };
            var activitySet = new ActivitySet
            {
                Activities = new[] { activity },
                Watermark = "watermark"
            };

            // WebChat expects the activities to be wrapped in an object with an activities property
            clientRequest.SetBody(activitySet);
            return await WebSocketServer.SendAsync(clientRequest, cancellationToken).ConfigureAwait(false);
        }

        private class ActivitySet
        {
            [JsonProperty("activities")]
            public Activity[] Activities { get; set; }

            [JsonProperty("watermark")]
            public string Watermark { get; set; }
        }

        private class StreamingConnectionFactory : ConnectorFactory
        {
            private readonly WebSocketServer _socketServer;
            private readonly ILogger _logger;

            public StreamingConnectionFactory(WebSocketServer socketServer, ILogger logger)
            {
                _socketServer = socketServer;
                _logger = logger;
            }

            public override Task<IConnectorClient> CreateAsync(string serviceUrl, string audience, CancellationToken cancellationToken)
            {
                var httpClient = new WebSocketHttpClient(_socketServer, _logger);
                return Task.FromResult<IConnectorClient>(new ConnectorClient(MicrosoftAppCredentials.Empty, httpClient, false));
            }
        }

        private class WebSocketHttpClient : HttpClient
        {
            private readonly WebSocketServer _socketServer;
            private readonly ILogger _logger;

            public WebSocketHttpClient(WebSocketServer socketServer, ILogger logger)
            {
                _socketServer = socketServer;
                _logger = logger;
            }

            public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
            {
                var streamingRequest = await CreateSteamingRequestAsync(httpRequestMessage).ConfigureAwait(false);
                var receiveResponse = await SendStreamingRequestAsync(streamingRequest, cancellationToken).ConfigureAwait(false);
                var httpResponseMessage = await CreateHttpResponseAsync(receiveResponse).ConfigureAwait(false);
                return httpResponseMessage;
            }

            private async Task<StreamingRequest> CreateSteamingRequestAsync(HttpRequestMessage httpRequestMessage)
            {
                var streamingRequest = new StreamingRequest
                {
                    Path = httpRequestMessage.RequestUri.OriginalString.Substring(httpRequestMessage.RequestUri.OriginalString.IndexOf("/conversation", StringComparison.Ordinal)),
                    Verb = httpRequestMessage.Method.ToString(),
                };

                // Stream client doesn't expect the path to contains any thing after activities
                streamingRequest.Path = streamingRequest.Path.Substring(0, streamingRequest.Path.IndexOf("activities", StringComparison.Ordinal) + "activities".Length);

                if (httpRequestMessage.Content != null)
                {
                    var contentString = await httpRequestMessage.Content.ReadAsStringAsync();
                    var activity = JsonConvert.DeserializeObject<Activity>(contentString);
                    activity.Timestamp = DateTime.UtcNow;
                    activity.Id = Guid.NewGuid().ToString();
                    activity.From = new ChannelAccount { Id = "bot", Name = "bot" };

                    var activitySet = new
                    {
                        activities = new Activity[] { activity },
                        watermark = "watermark"
                    };

                    streamingRequest.SetBody(activitySet);
                }
                return streamingRequest;
            }

            private async Task<ReceiveResponse> SendStreamingRequestAsync(StreamingRequest request, CancellationToken cancellationToken)
            {
                ReceiveResponse receiveResponse = new ReceiveResponse() { StatusCode = (int)HttpStatusCode.OK };
                if (_socketServer.IsConnected)
                {
                    var response = await _socketServer.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    receiveResponse.StatusCode = response.StatusCode;
                }
                return receiveResponse;
            }

            private async Task<HttpResponseMessage> CreateHttpResponseAsync(ReceiveResponse receiveResponse)
            {
                var httpResponseMessage = new HttpResponseMessage((HttpStatusCode)receiveResponse.StatusCode);
                httpResponseMessage.Content = new StringContent(await receiveResponse.ReadBodyAsStringAsync().ConfigureAwait(false));
                return httpResponseMessage;
            }
        }

    }
}
