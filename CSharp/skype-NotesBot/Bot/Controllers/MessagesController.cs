using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Bot.Dialogs;
using Bot.Models;
using Bot.Telemetry;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private IMongoDatabase _database;
        private static IConnectorClient _connectorClient;

        public MessagesController()
        {
            _database = new MongoClient(BotConstants.DbString).GetDatabase(BotConstants.DbName);
        }

        public MessagesController(IMongoDatabase dB, IConnectorClient connectorClient)
        {
            _database = dB;
            _connectorClient = connectorClient;
        }

        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            var responseMessage = string.Empty;
            try
            {
                if (_database == null)
                {
                    _database = new MongoClient(BotConstants.DbString).GetDatabase(BotConstants.DbName);
                }

                if (activity.Type == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => new RootDialog());
                }
                else
                {
                    responseMessage = await OnHandleSystemMessage(activity);
                }
            }
            catch (Exception)
            {
                // No action.
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            return response;
        }

        private async Task<string> OnHandleSystemMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.ContactRelationUpdate:
                    return await OnContactRelationUpdate(activity);
                case ActivityTypes.ConversationUpdate:
                    return await OnConversationUpdate(activity);
            }

            TelemetryLogger.TrackActivity(activity);
            return string.Empty;
        }

        private async Task<string> OnContactRelationUpdate(Activity activity)
        {
            switch (activity.Action)
            {
                case ContactRelationUpdateActionTypes.Add:
                    return await OnAddUser(activity);
                case ContactRelationUpdateActionTypes.Remove:
                    OnRemoveUser(activity);
                    break;
            }

            return null;
        }

        private async Task<string> OnAddUser(Activity activity)
        {
            var user = await OnGetUser(activity.From.Id);
            if (user?.IsFriend == true)
            {
                return null;
            }

            if (!user?.IsFriend == true)
            {
                return await OnAddRevistingUser(activity, user.UserName);
            }

            return await OnAddBrandNewUser(activity);
        }

        private async Task<string> OnAddRevistingUser(Activity activity, string userName)
        {
            var collection = _database.GetCollection<User>(BotConstants.AllUsers);
            var filter = Builders<User>.Filter.Where(x => x.UserId == activity.From.Id);
            var update = Builders<User>.Update.Set(x => x.IsFriend, new BsonBoolean(true));
            await collection.FindOneAndUpdateAsync(filter, update);

            var welcomeCard = new HeroCard
            {
                Title = $"Hello {userName}, good to have you back.",
                Text = "I am here to be your assistant and take notes for you.",
                Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Url = BotConstants.WelcomeCardLink
                    }
                },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Help", value: BotConstants.Help)
                }
            };

            return await OnSendMessageWithAttachment(activity, new List<Attachment> {welcomeCard.ToAttachment()});
        }

        private async Task<string> OnAddBrandNewUser(Activity activity)
        {
            var user = new User
            {
                DocumentId = activity.From.Id,
                UserId = activity.From.Id,
                UserName = activity.From.Name,
                BotId = activity.Recipient.Id,
                BotName = activity.Recipient.Name,
                ServiceUrl = activity.ServiceUrl,
                ChannelId = activity.ChannelId,
                ConversationId = activity.Conversation.Id,
                GroupIds = new List<string>(),
                IsFriend = true,
                TaggedNotes = new List<TaggedNote>(),
                SavedNotes = new List<Note>()
            };

            var collection = _database.GetCollection<User>(BotConstants.AllUsers);
            await collection.InsertOneAsync(user);
            var welcomeCard = new HeroCard
            {
                Title = $"Hello {user.UserName}, I am the Notes bot.",
                Text = "I am here to be your assistant and take notes for you.",
                Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Url = BotConstants.WelcomeCardLink
                    }
                },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Help", value: BotConstants.Help)
                }
            };

            return await OnSendMessageWithAttachment(activity, new List<Attachment> {welcomeCard.ToAttachment()});
        }

        private async void OnRemoveUser(IActivity activity)
        {
            var collection = _database.GetCollection<User>(BotConstants.AllUsers);
            var filter = Builders<User>.Filter.Where(x => x.UserId == activity.From.Id);
            var update = Builders<User>.Update.Set(x => x.IsFriend, false);
            await collection.FindOneAndUpdateAsync(filter, update);
        }

        private static async Task<string> OnConversationUpdate(Activity activity)
        {
            var resourceResponse = string.Empty;
            var membersAdded = activity.MembersAdded;
            if (membersAdded?.Any() == true)
            {
                await OnMembersAdded(activity);
            }
            return resourceResponse;
        }

        private static async Task<string> OnMembersAdded(Activity activity)
        {
            var resourceResponse = string.Empty;
            var isBotGotAdded = activity.MembersAdded.Any(channelAccount => channelAccount.Id == activity.Recipient.Id);

            if (!isBotGotAdded)
            {
                return resourceResponse;
            }

            var welcomeCard = new HeroCard
            {
                Title = "Hi, I am the Notes bot.",
                Text = "I am here to be your assistant and take notes for you.",
                Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Url = BotConstants.WelcomeCardLink
                    }
                },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Help", value: BotConstants.Help)
                }
            };

            resourceResponse =
                await OnSendMessageWithAttachment(activity, new List<Attachment> {welcomeCard.ToAttachment()});
            return resourceResponse;
        }

        private static async Task<string> OnSendMessageWithAttachment(Activity activity,
            IList<Attachment> attachments)
        {
            var reply = activity.CreateReply();
            reply.Attachments = attachments;
            if (_connectorClient == null)
            {
                _connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));
            }

            var resourceResponse = await _connectorClient.Conversations.SendToConversationAsync(reply);
            return resourceResponse.Id;
        }

        private async Task<User> OnGetUser(string userId)
        {
            User user = null;
            var userCollection = _database.GetCollection<User>(BotConstants.AllUsers);
            var filter = Builders<User>.Filter.Where(x => x.UserId == userId);
            var userCursor = await userCollection.FindAsync(filter);
            while (await userCursor.MoveNextAsync())
            {
                var listOfUsers = userCursor.Current;
                var users = listOfUsers as IList<User> ?? listOfUsers.ToList();
                user = !users.Any() ? null : users.First();
            }

            return user;
        }
    }
}