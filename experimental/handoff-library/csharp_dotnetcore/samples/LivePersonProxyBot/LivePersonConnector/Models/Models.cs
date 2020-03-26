using System;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace LivePersonConnector.Models
{
    public class DomainInfo
    {
        public string service { get; set; }
        public string account { get; set; }
        public string baseURI { get; set; }
    }

    public class AppJWT
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class ConsumerId
    {
        public string ext_consumer_id { get; set; }
    }


    public class ConsumerJWS
    {
        public string token { get; set; }
    }

    // Start conversation
    public class Conversation
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string skillId { get; set; }
        public Body body { get; set; }
    }

    public class Body
    {
        public Authenticateddata authenticatedData { get; set; }
        public string brandId { get; set; }
    }

    public class Authenticateddata
    {
        public Lp_Sdes[] lp_sdes { get; set; }
    }

    public class Lp_Sdes
    {
        public string type { get; set; }
        public Info info { get; set; }
        public Personal personal { get; set; }
    }

    public class Info
    {
        public string socialId { get; set; }
        public string ctype { get; set; }
    }

    public class Personal
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string gender { get; set; }
    }

    public class ConversationResponse
    {
        public string code { get; set; }
        public ConversationResponseBody body { get; set; }
        public string reqId { get; set; }
    }

    public class ConversationResponseBody
    {
        public string msg { get; set; }
        public string conversationId { get; set; }
    }

    // Send conversation

    public class Message
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public MessageBody body { get; set; }
    }

    public class MessageBody
    {
        public string dialogId { get; set; }
        public MessageBodyEvent @event { get; set; }
    }

    public class MessageBodyEvent
    {
        public string type { get; set; }
        public string contentType { get; set; }
        public string message { get; set; }
    }

    public class SendResponse
    {
        public string reqId { get; set; }
        public string code { get; set; }
        public SendBody body { get; set; }
    }

    public class SendBody
    {
        public int sequence { get; set; }
    }

    // Webhook models:

    namespace Webhook
    {
        public class WebhookData
        {
            public string kind { get; set; }
            public Body body { get; set; }
            public string type { get; set; }
        }

        public class Body
        {
            public Change[] changes { get; set; }
        }

        public class Change
        {
            public int sequence { get; set; }
            public string originatorId { get; set; }
            public Originatormetadata originatorMetadata { get; set; }
            public long serverTimestamp { get; set; }
            public Event @event { get; set; }
            public string conversationId { get; set; }
            public string dialogId { get; set; }
            public Result result { get; set; }
        }

        public class Originatormetadata
        {
            public string id { get; set; }
            public string role { get; set; }
        }

        public class Event
        {
            public string type { get; set; }
            public string chatState { get; set; }
            public string message { get; set; }
            public string contentType { get; set; }
        }

        public class Result
        {
            public string convId { get; set; }
            public int effectiveTTR { get; set; }
            public Conversationdetails conversationDetails { get; set; }
        }

        public class Conversationdetails
        {
            public string skillId { get; set; }
            public Participant[] participants { get; set; }
            public Dialog[] dialogs { get; set; }
            public string brandId { get; set; }
            public string state { get; set; }
            public string stage { get; set; }
            public string closeReason { get; set; }
            public long startTs { get; set; }
            public long endTs { get; set; }
            public long metaDataLastUpdateTs { get; set; }
            public Ttr ttr { get; set; }
            public Conversationhandlerdetails conversationHandlerDetails { get; set; }
        }

        public class Ttr
        {
            public string ttrType { get; set; }
            public int value { get; set; }
        }

        public class Conversationhandlerdetails
        {
            public string accountId { get; set; }
            public string skillId { get; set; }
        }

        public class Participant
        {
            public string id { get; set; }
            public string role { get; set; }
        }

        public class Dialog
        {
            public string dialogId { get; set; }
            public Participantsdetail[] participantsDetails { get; set; }
            public string dialogType { get; set; }
            public string channelType { get; set; }
            public Metadata metaData { get; set; }
            public string state { get; set; }
            public long creationTs { get; set; }
            public long endTs { get; set; }
            public long metaDataLastUpdateTs { get; set; }
            public string closedBy { get; set; }
            public string closedCause { get; set; }
        }

        public class Metadata
        {
            public string appInstallId { get; set; }
        }

        public class Participantsdetail
        {
            public string id { get; set; }
            public string role { get; set; }
            public string state { get; set; }
        }
    }

    namespace ChatStateEvent
    {
        public class WebhookData
        {
            public string kind { get; set; }
            public Body body { get; set; }
            public string type { get; set; }
        }

        public class Body
        {
            public Change[] changes { get; set; }
        }

        public class Change
        {
            public string originatorId { get; set; }
            public Originatormetadata originatorMetadata { get; set; }
            public Event @event { get; set; }
            public string conversationId { get; set; }
            public string dialogId { get; set; }
        }

        public class Originatormetadata
        {
            public string id { get; set; }
            public string role { get; set; }
        }

        public class Event
        {
            public string type { get; set; }
            public string chatState { get; set; }
        }
    }

    namespace AcceptStatusEvent
    {
        public class WebhookData
        {
            public string kind { get; set; }
            public Body body { get; set; }
            public string type { get; set; }
        }

        public class Body
        {
            public Change[] changes { get; set; }
        }

        public class Change
        {
            public int sequence { get; set; }
            public string originatorId { get; set; }
            public Originatormetadata originatorMetadata { get; set; }
            public long serverTimestamp { get; set; }
            public Event _event { get; set; }
            public string conversationId { get; set; }
            public string dialogId { get; set; }
        }

        public class Originatormetadata
        {
            public string id { get; set; }
            public string role { get; set; }
        }

        public class Event
        {
            public string type { get; set; }
            public string status { get; set; }
            public int[] sequenceList { get; set; }
        }
    }

    namespace ExConversationChangeNotification
    {
        public class WebhookData
        {
            public string kind { get; set; }
            public Body body { get; set; }
            public string type { get; set; }
        }

        public class Body
        {
            public long sentTs { get; set; }
            public Change[] changes { get; set; }
        }

        public class Change
        {
            public string type { get; set; }
            public Result result { get; set; }
        }

        public class Result
        {
            public string convId { get; set; }
            public long effectiveTTR { get; set; }
            public Conversationdetails conversationDetails { get; set; }
        }

        public class Conversationdetails
        {
            public string skillId { get; set; }
            public Participant[] participants { get; set; }
            public Dialog[] dialogs { get; set; }
            public string brandId { get; set; }
            public string state { get; set; }
            public string stage { get; set; }
            public long startTs { get; set; }
            public long metaDataLastUpdateTs { get; set; }
            public Ttr ttr { get; set; }
            public Conversationhandlerdetails conversationHandlerDetails { get; set; }
        }

        public class Ttr
        {
            public string ttrType { get; set; }
            public int value { get; set; }
        }

        public class Conversationhandlerdetails
        {
            public string accountId { get; set; }
            public string skillId { get; set; }
        }

        public class Participant
        {
            public string id { get; set; }
            public string role { get; set; }
        }

        public class Dialog
        {
            public string dialogId { get; set; }
            public Participantsdetail[] participantsDetails { get; set; }
            public string dialogType { get; set; }
            public string channelType { get; set; }
            public Metadata metaData { get; set; }
            public string state { get; set; }
            public long creationTs { get; set; }
            public long metaDataLastUpdateTs { get; set; }
        }

        public class Metadata
        {
            public string appInstallId { get; set; }
        }

        public class Participantsdetail
        {
            public string id { get; set; }
            public string role { get; set; }
            public string state { get; set; }
        }

    }
}

