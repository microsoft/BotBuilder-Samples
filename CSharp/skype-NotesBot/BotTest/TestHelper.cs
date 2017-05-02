using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using MongoDB.Driver;
using Moq;

namespace BotTest
{
    internal static class TestHelper
    {
        private static IAwaitable<T> CreateAwaitable<T>(T result)
        {
            var awaiter = new Mock<IAwaiter<T>>();
            awaiter.Setup(a => a.GetResult()).Returns(() => result);
            awaiter.SetupGet(a => a.IsCompleted).Returns(() => true);

            var awaitable = new Mock<IAwaitable<T>>();
            awaitable.Setup(a => a.GetAwaiter()).Returns(() => awaiter.Object);

            return awaitable.Object;
        }

        internal static IAwaitable<IMessageActivity> CreateAwaitableMessage(string text)
        {
            return CreateAwaitable<IMessageActivity>(new Activity {Text = text});
        }

        internal static Task<IAsyncCursor<User>> SetupResultInFirstBatch(User user)
        {
            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));
            mockCursor.SetupSequence(c => c.Current).Returns(new List<User> {user}).Returns(null);
            return Task.FromResult(mockCursor.Object);
        }

        internal static User OnGetTestUserWithTaggedAndSavedNotes()
        {
            var user = new User
            {
                DocumentId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                UserId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                UserName = "Test Account",
                BotId = "<your bot id>",
                BotName = "<your bot name>",
                ServiceUrl = "https://smba.trafficmanager.net/apis/",
                ChannelId = "skype",
                ConversationId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                GroupIds = new List<string>(),
                IsFriend = true,
                TaggedNotes = new List<TaggedNote>(),
                SavedNotes = new List<Note>()
            };

            var taggedNote = new TaggedNote
            {
                TagValue = "abcd",
                NoteIds = new List<string> {"29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB_636263307674234489"}
            };

            var note = new Note
            {
                DocumentId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB_636263307674234489",
                NoteOwnerId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                NoteContent = "this is a note with #abcd",
                TimestampTicks = 636263307674234489,
                HashTagsUsed = new List<string> {"abcd"}
            };

            user.TaggedNotes.Add(taggedNote);
            user.SavedNotes.Add(note);
            return user;
        }

        internal static User OnGetTestUserWithoutTaggedAndSavedNotes()
        {
            var user = new User
            {
                DocumentId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                UserId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                UserName = "Test Account",
                BotId = "<your bot id>",
                BotName = "<your bot name>",
                ServiceUrl = "https://smba.trafficmanager.net/apis/",
                ChannelId = "skype",
                ConversationId = "29:1uapLqaLh3qpLYqlEOGwUjkLwUwzGKjmzsT6jS6p7kbB",
                GroupIds = new List<string>(),
                IsFriend = true,
                TaggedNotes = new List<TaggedNote>(),
                SavedNotes = new List<Note>()
            };

            return user;
        }
    }
}