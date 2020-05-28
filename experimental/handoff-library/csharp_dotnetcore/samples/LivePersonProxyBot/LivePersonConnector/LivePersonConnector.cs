// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LivePersonConnector.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LivePersonConnector
{
    public interface ILivePersonCredentialsProvider
    {
        string LpAccount { get; }
        string LpAppId { get; }
        string LpAppSecret { get; }
        string MsAppId { get; }
    }

    // Refer to https://developers.liveperson.com/connector-api-send-api-authorization-and-authentication.html
    // for definition of AppJWT and ConsumerJWS
    internal class LivePersonConversationRecord
    {
        public string ConversationId { get; set; }
        public string MessageDomain { get; set; }
        public string AppJWT { get; set; }
        public string ConsumerJWS { get; set; }
    }

    static class LivePersonConnector
    {
        static public async Task<LivePersonConversationRecord> EscalateToAgenAsync(ITurnContext turnContext, IEventActivity handoffEvent, string account, string clientId, string clientSecret, ConversationMap conversationMap)
        {
            var sentinelDomain = await GetDomainAsync(account, "sentinel").ConfigureAwait(false);
            var appJWT = await GetAppJWTAsync(account, sentinelDomain, clientId, clientSecret).ConfigureAwait(false);
            var consumer = new ConsumerId { ext_consumer_id = turnContext.Activity.From.Id };

            var idpDomain = await GetDomainAsync(account, "idp").ConfigureAwait(false);
            var consumerJWS = await GetConsumerJWSAsync(account, idpDomain, appJWT, consumer).ConfigureAwait(false);

            var context = handoffEvent.Value as JObject;

            // This can be null:
            var skill = context?.Value<string>("Skill");
            var engagementAttributes = (context["EngagementAttributes"] as JArray)?.ToObject<EngagementAttribute[]>();

            var msgDomain = await GetDomainAsync(account, "asyncMessagingEnt").ConfigureAwait(false);
            var conversations = new Conversation[] {
                    new Conversation {
                        kind = "req",
                        id = "1,",
                        type = "userprofile.SetUserProfile",
                        body = new Body { authenticatedData = new Authenticateddata {
                            lp_sdes = engagementAttributes?.Select(ea => ea.ToLivePersonEngagementAttribute()).ToArray()
                        }}},
                    new Conversation {
                        kind = "req",
                        id = "2,",
                        type = "cm.ConsumerRequestConversation",
                        skillId = skill,
                        body = new Body { brandId = account }
                    },
            };

            var conversationId = await StartConversationAsync(account, msgDomain, appJWT, consumerJWS, conversations).ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine($"Started LP conversation id {conversationId}");

            conversationMap.ConversationRecords.TryAdd(conversationId, new ConversationRecord { ConversationReference = turnContext.Activity.GetConversationReference() });

            var messageId = 1;

            // First, play out the transcript
            var handoffActivity = handoffEvent as Activity;
            if (handoffActivity.Attachments != null)
            {
                foreach (var attachment in handoffActivity.Attachments)
                {
                    if (attachment.Name == "Transcript")
                    {
                        var transcript = attachment.Content as Transcript;
                        foreach (var activity in transcript.Activities)
                        {
                            var message2 = MakeLivePersonMessage(messageId++,
                                conversationId,
                                $"{activity.From.Name}: {activity.Text}");
                            await SendMessageToConversationAsync(account, msgDomain, appJWT, consumerJWS, message2).ConfigureAwait(false);
                        }
                    }
                }
            }

            return new LivePersonConversationRecord { ConversationId = conversationId, AppJWT = appJWT, ConsumerJWS = consumerJWS, MessageDomain = msgDomain };
        }

        static public Message MakeLivePersonMessage(int id, string conversationId, string text)
        {
            return new Message
            {
                kind = "req",
                id = id.ToString(),
                type = "ms.PublishEvent",
                body = new MessageBody
                {
                    dialogId = conversationId,
                    @event = new MessageBodyEvent
                    {
                        type = "ContentEvent",
                        contentType = "text/plain",
                        message = text
                    }
                }
            };
        }

        static private async Task<string> GetDomainAsync(string account, string serviceName)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"http://api.liveperson.net/api/account/{account}/service/{serviceName}/baseURI.json?version=1.0").ConfigureAwait(false);
                if (result.IsSuccessStatusCode)
                {
                    var strResult = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var domain = JsonConvert.DeserializeObject<DomainInfo>(strResult);
                    return domain.baseURI;
                }
                else
                {
                    throw new Exception($"Failed to get Domain for service {serviceName}. Error {result.StatusCode}");
                }
            }
        }

        static private async Task<string> GetAppJWTAsync(string account, string domain, string clientId, string clientSecret)
        {
            using (var client = new HttpClient())
            {
                var stringPayload = "";
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/x-www-form-urlencoded");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = httpContent,
                    RequestUri = new Uri($"https://{domain}/sentinel/api/account/{account}/app/token?v=1.0&grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}")
                };

                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var strResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var appJWT = JsonConvert.DeserializeObject<AppJWT>(strResult);
                    return appJWT.access_token;
                }
                else
                {
                    throw new Exception($"Failed to obtain AppJWT");
                }
            }
        }

        static private async Task<string> GetConsumerJWSAsync(string account, string domain, string authToken, ConsumerId consumer)
        {
            using (var client = new HttpClient())
            {
                var stringPayload = JsonConvert.SerializeObject(consumer);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = httpContent
                };
                request.Headers.Add("Authorization", authToken);
                request.RequestUri = new Uri($"https://{domain}/api/account/{account}/consumer?v=1.0");

                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var strResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var consumerJWS = JsonConvert.DeserializeObject<ConsumerJWS>(strResult);
                    return consumerJWS.token;
                }
                else
                {
                    throw new Exception($"Failed to obtain ConsumerJWS");
                }
            }
        }

        static private async Task<string> StartConversationAsync(string account, string domain, string appJWT, string consumerJWS, Conversation[] conversations)
        {
            using (var client = new HttpClient())
            {
                var stringPayload = JsonConvert.SerializeObject(conversations, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = httpContent
                };
                request.Headers.Add("Authorization", appJWT);
                request.Headers.Add("X-LP-ON-BEHALF", consumerJWS);
                request.RequestUri = new Uri($"https://{domain}/api/account/{account}/messaging/consumer/conversation?v=3");

                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var strResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var conversationResponses = JsonConvert.DeserializeObject<ConversationResponse[]>(strResult);
                    foreach (var convo in conversationResponses)
                    {
                        var convId = convo.body?.conversationId;
                        if (convId != null)
                        {
                            return convId;
                        }
                    }

                    throw new Exception($"Failed to StartConversation - cannot get conversation id");
                }
                else
                {
                    throw new Exception($"Failed to StartConversation");
                }
            }
        }

        static public async Task<int> SendMessageToConversationAsync(string account, string domain, string appJWT, string consumerJWS, Message message)
        {
            using (var client = new HttpClient())
            {
                var stringPayload = JsonConvert.SerializeObject(message, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = httpContent
                };
                request.Headers.Add("Authorization", appJWT);
                request.Headers.Add("X-LP-ON-BEHALF", consumerJWS);
                request.RequestUri = new Uri($"https://{domain}/api/account/{account}/messaging/consumer/conversation/send?v=3");

                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var strResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var sendResponse = JsonConvert.DeserializeObject<SendResponse>(strResult);
                    return sendResponse.body.sequence;
                }
                else
                {
                    throw new Exception($"Failed to send message to conversation. Response code {response.StatusCode}");
                }
            }
        }
    }
}
