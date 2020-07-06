// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace LivePersonConnector.Controllers
{
    [Route("api/LivePerson")]
    [ApiController]
    public class LivePersonController : ControllerBase
    {
        private readonly ILivePersonAdapter _adapter;
        private readonly IBot _bot;
        private readonly ILivePersonCredentialsProvider _creds;

        // This must be a durable storage in multi-instance scenario
        private readonly ConversationMap _conversationMap;

        public LivePersonController(BotFrameworkHttpAdapter adapter, ILivePersonCredentialsProvider creds, IBot bot, ConversationMap conversationMap)
        {
            _adapter = (ILivePersonAdapter)adapter;
            _bot = bot;
            _creds = creds;
            _conversationMap = conversationMap;
        }

        private bool Authenticate(HttpRequest request, string body)
        {
            // https://github.com/LivePersonInc/developers-community/blob/ae8890694cb9b3be797ca382e9ad7395382aed25/pages/documents/MessagingChannels/ConnectorAPI/webhooks/security.md#authentication
            if (!request.Headers.ContainsKey("X-Liveperson-Signature") || !request.Headers.ContainsKey("X-Liveperson-Client-Id") || !request.Headers.ContainsKey("X-Liveperson-Account-Id"))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }

            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_creds.LpAppSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(body));
                var signature = $"sha1={Convert.ToBase64String(hash)}";
                if (signature != request.Headers["X-Liveperson-Signature"])
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return false;
                }
            }

            var account = request.Headers["X-Liveperson-Account-Id"];
            var clientId = request.Headers["X-Liveperson-Client-Id"];
            if (account != _creds.LpAccount || clientId != _creds.LpAppId)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return false;
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return true;
        }

        [HttpPost]
        [Route("AcceptStatusEvent")]
        public async Task PostAcceptStatusEventAsync()
        {
            using (var readStream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await readStream.ReadToEndAsync();

                if (!Authenticate(Request, body)) return;

                try
                {
                    var wbhookData = JsonConvert.DeserializeObject<Models.AcceptStatusEvent.WebhookData>(body);
                    foreach (var change in wbhookData.body.changes)
                    {
                        if (change?.originatorMetadata?.role == "ASSIGNED_AGENT")
                        {
                            // Agent has accepted the conversation
                            var convId = change?.conversationId;
                            ConversationRecord conversationRec;
                            if (_conversationMap.ConversationRecords.TryGetValue(convId, out conversationRec))
                            {
                                if (conversationRec.IsAcknowledged || conversationRec.IsClosed)
                                {
                                    // Already acked this one
                                    break;
                                }

                                var newConversationRec = new ConversationRecord
                                {
                                    ConversationReference = conversationRec.ConversationReference,
                                    IsClosed = conversationRec.IsClosed,
                                    IsAcknowledged = true
                                };

                                // Update atomically -- only one will succeed
                                if (_conversationMap.ConversationRecords.TryUpdate(convId, newConversationRec, conversationRec))
                                {
                                    var evnt = EventFactory.CreateHandoffStatus(newConversationRec.ConversationReference.Conversation, "accepted") as Activity;
                                    await _adapter.ProcessActivityAsync(evnt, _creds.MsAppId, newConversationRec.ConversationReference, _bot.OnTurnAsync, default(CancellationToken));
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        [HttpPost]
        [Route("chatstateevent")]
        public async Task PostChatStateEventAsync()
        {
            using (var readStream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await readStream.ReadToEndAsync();

                if (!Authenticate(Request, body)) return;

                try
                {
                    var wbhookData = JsonConvert.DeserializeObject<Models.ChatStateEvent.WebhookData>(body);
                    foreach (var change in wbhookData.body.changes)
                    {
                        if (change?.@event?.chatState == "COMPOSING")
                        {
                            var convId = change.conversationId;
                            ConversationRecord conversationRec;
                            if (_conversationMap.ConversationRecords.TryGetValue(convId, out conversationRec))
                            {
                                var evnt = EventFactory.CreateHandoffStatus(conversationRec.ConversationReference.Conversation, "typing") as Activity;
                                await _adapter.ProcessActivityAsync(evnt, _creds.MsAppId, conversationRec.ConversationReference, _bot.OnTurnAsync, default(CancellationToken));
                            }
                        }
                    }
                }
                catch { }
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        [HttpPost]
        [Route("contentevent")]
        public async Task PostContentEventAsync()
        {
            using (var readStream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await readStream.ReadToEndAsync();

                if (!Authenticate(Request, body)) return;

                var wbhookData = JsonConvert.DeserializeObject<Models.Webhook.WebhookData>(body);

                foreach (var change in wbhookData.body.changes)
                {
                    if (change?.@event.type == "ContentEvent" && change?.originatorMetadata?.role == "ASSIGNED_AGENT")
                    {
                        if (change.@event.message != null)
                        {
                            var humanActivity = MessageFactory.Text(change.@event.message);

                            ConversationRecord conversationRec;
                            if (_conversationMap.ConversationRecords.TryGetValue(change.conversationId, out conversationRec))
                            {
                                if (!conversationRec.IsClosed)
                                {
                                    MicrosoftAppCredentials.TrustServiceUrl(conversationRec.ConversationReference.ServiceUrl);

                                    await ((BotFrameworkHttpAdapter)_adapter).ContinueConversationAsync(
                                        _creds.MsAppId,
                                        conversationRec.ConversationReference,
                                        (ITurnContext turnContext, CancellationToken cancellationToken) =>
                                            turnContext.SendActivityAsync(humanActivity, cancellationToken),
                                        default(CancellationToken));
                                }
                            }
                            else
                            {
                                // The bot has no record of this conversation, this should not happen
                                throw new Exception("Cannot find conversation");
                            }
                        }
                    }
                }
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        [HttpPost]
        [Route("richcontentevent")]
        public async Task PostRichContentEventAsync()
        {
            using (var readStream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await readStream.ReadToEndAsync();

                if (!Authenticate(Request, body)) return;

                try
                {
                    var wbhookData = JsonConvert.DeserializeObject<Models.Webhook.WebhookData>(body);
                }
                catch { }
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
        }

        [HttpPost]
        [Route("ExConversationChangeNotification")]
        public async Task PostExConversationChangeNotification()
        {
            using (var readStream = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await readStream.ReadToEndAsync();

                if (!Authenticate(Request, body)) return;

                try
                {
                    var wbhookData = JsonConvert.DeserializeObject<Models.ExConversationChangeNotification.WebhookData>(body);

                    foreach (var change in wbhookData.body.changes)
                    {
                        string state = change?.result?.conversationDetails?.state;
                        switch (state)
                        {
                            case "CLOSE":
                            {
                                // Agent has closed the conversation
                                var convId = change?.result?.convId;
                                ConversationRecord conversationRec;
                                if (_conversationMap.ConversationRecords.TryGetValue(convId, out conversationRec))
                                {
                                    var evnt = EventFactory.CreateHandoffStatus(conversationRec.ConversationReference.Conversation, "completed") as Activity;
                                    await _adapter.ProcessActivityAsync(evnt, _creds.MsAppId, conversationRec.ConversationReference, _bot.OnTurnAsync, default(CancellationToken));

                                    // Close event happens only once, so don't worry about race conditions here
                                    // Records are not removed from the dictionary since agents can reopen conversations
                                    conversationRec.IsClosed = true;
                                }
                            }
                            break;
                            case "OPEN":
                                break;
                        }
                    }
                }
                catch { }
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
        }
    }
}
