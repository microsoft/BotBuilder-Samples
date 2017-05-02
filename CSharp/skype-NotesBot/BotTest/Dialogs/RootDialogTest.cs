using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Bot;
using Bot.Dialogs;
using Bot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace BotTest.Dialogs
{
    [TestClass]
    public sealed class RootDialogTest : DialogTestBase
    {
        [Test]
        [TestCase("", false)]
        [TestCase("hi", false)]
        [TestCase("help1", false)]
        [TestCase("note", true)]
        [TestCase("show", true)]
        [TestCase("delete", true)]
        [TestCase("delete force", true)]
        [TestCase("export", true)]
        [TestCase("export email", true)]
        [TestCase("help", true)]
        [TestCase("   note   ", true)]
        [TestCase("   show   ", true)]
        [TestCase("   delete   ", true)]
        [TestCase("   delete force   ", true)]
        [TestCase("   export   ", true)]
        [TestCase("   export email   ", true)]
        [TestCase("   help   ", true)]
        public void CheckStringIsValidCommand(string commandString, bool expected)
        {
            // Arrange
            var rootDialog = new RootDialog();
            var args = new object[] {commandString};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            var isValidEmail = privateObject.Invoke("OnCheckIsStringValidCommand", args);

            // Assert
            Assert.AreEqual(expected, isValidEmail);
        }

        [Test]
        [TestCase("help abcd", false)]
        [TestCase("help help", false)]
        [TestCase("hepl note", false)]
        [TestCase("help show note", false)]
        [TestCase("help", true)]
        [TestCase("help note", true)]
        [TestCase("help show", true)]
        [TestCase("help delete", true)]
        [TestCase("help export", true)]
        public void CheckStringIsValidHelpCommand(string commandString, bool expected)
        {
            // Arrange
            var rootDialog = new RootDialog();
            var allWords = commandString.Split(' ')
                .Where(word => !string.IsNullOrEmpty(word))
                .Select(word => word.Replace(" ", ""))
                .ToList();
            var args = new object[] {allWords};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            var isValidEmail = privateObject.Invoke("OnCheckIsStringValidHelpCommand", args);

            // Assert
            Assert.AreEqual(expected, isValidEmail);
        }

        [Test]
        [TestCase("note abcd")]
        public void CheckOnAddNoteCommandMethod(string message)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithTaggedAndSavedNotes();

            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);

            var collection = new Mock<IMongoCollection<User>>();
            collection.Object.InsertOneAsync(user);
            collection.Setup(a => a.FindOneAndUpdateAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<UpdateDefinition<User>>(),
                    It.IsAny<FindOneAndUpdateOptions<User>>(),
                    CancellationToken.None))
                .Returns(Task.FromResult(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);

            var rootDialog = new RootDialog(mongoDatabase.Object);
            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var messageActivity = context.Object.MakeMessage();
            messageActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            context.Verify(
                c =>
                    c.PostAsync(It.Is<IMessageActivity>(a => a.AttachmentLayout == AttachmentLayoutTypes.Carousel),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [TestCase("show", "You have **1 notes**:")]
        [TestCase("show #abcd", "**1 notes** of your 1 notes contain the string **abcd**:")]
        [TestCase("show this", "**1 notes** of your 1 notes contain the string **this**:")]
        public void CheckOnShowCommandMethod(string message, string expected)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithTaggedAndSavedNotes();
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var collection = new Mock<IMongoCollection<User>>();
            collection.Setup(a => a.FindAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(TestHelper.SetupResultInFirstBatch(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);
            var rootDialog = new RootDialog(mongoDatabase.Object);

            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            context.Verify(
                c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [TestCase("delete", BotConstants.NoNotesToDelete)]
        [TestCase("delete force", BotConstants.NoNotesToDelete)]
        public void CheckOnDeleteCommandMethodWithoutNotes(string message, string expected)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithoutTaggedAndSavedNotes();
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var collection = new Mock<IMongoCollection<User>>();
            collection.Setup(a => a.FindAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(TestHelper.SetupResultInFirstBatch(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);
            var rootDialog = new RootDialog(mongoDatabase.Object);

            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            context.Verify(
                c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [TestCase("delete")]
        public void CheckOnDeleteCommandMethodWitNotes(string message)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithTaggedAndSavedNotes();
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var collection = new Mock<IMongoCollection<User>>();
            collection.Setup(a => a.FindAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(TestHelper.SetupResultInFirstBatch(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);
            var rootDialog = new RootDialog(mongoDatabase.Object);

            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            context.Verify(
                c =>
                    c.PostAsync(It.Is<IMessageActivity>(a => a.AttachmentLayout == AttachmentLayoutTypes.Carousel),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [TestCase("delete force", "All of your **1 notes** have been **permanently deleted**.")]
        public void CheckOnDeleteForceCommandMethodWitNotes(string message, string expected)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithTaggedAndSavedNotes();
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var collection = new Mock<IMongoCollection<User>>();
            collection.Setup(a => a.FindAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(TestHelper.SetupResultInFirstBatch(user));
            collection.Setup(a => a.FindOneAndUpdateAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<UpdateDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(Task.FromResult(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);
            var rootDialog = new RootDialog(mongoDatabase.Object);

            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            context.Verify(
                c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [TestCase("export", BotConstants.NoNotesToExport)]
        [TestCase("export email", BotConstants.NoNotesToExport)]
        public void CheckOnExportCommandMethodWithoutNotes(string message, string expected)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithoutTaggedAndSavedNotes();
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var collection = new Mock<IMongoCollection<User>>();
            collection.Setup(a => a.FindAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(TestHelper.SetupResultInFirstBatch(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);
            var rootDialog = new RootDialog(mongoDatabase.Object);

            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);

            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            context.Verify(
                c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [TestCase("export")]
        [TestCase("export email")]
        public async Task CheckOnExportCommandMethodWithNotes(string message)
        {
            // Arrange
            var user = TestHelper.OnGetTestUserWithTaggedAndSavedNotes();
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var collection = new Mock<IMongoCollection<User>>();
            collection.Setup(a => a.FindAsync<User>(
                    It.IsAny<FilterDefinition<User>>(),
                    null,
                    CancellationToken.None))
                .Returns(TestHelper.SetupResultInFirstBatch(user));
            mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                .Returns(collection.Object);
            var rootDialog = new RootDialog(mongoDatabase.Object);

            IBotDataBag dataBag;
            var container = Build(Options.LastWriteWinsCachingBotDataStore);
            var msg = MakeTestMessage();
            using (var scope = DialogModule.BeginLifetimeScope(container, msg))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(default(CancellationToken));
                dataBag = scope.Resolve<Func<IBotDataBag>>()();
            }

            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.UserData).Returns(() => dataBag);
            context.Setup(c => c.Activity.From).Returns(() => new ChannelAccount(user.UserId, user.UserName));
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var args = new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)};
            var privateObject = new PrivateObject(rootDialog);
            context.Object.UserData.SetValue(BotConstants.AllNotesAsString, string.Empty);
            // Act
            privateObject.Invoke("OnMessageReceivedAsync", args);

            // Assert
            switch (message)
            {
                case "export email":
                    var noteString = context.Object.UserData.GetValue<string>(BotConstants.AllNotesAsString);
                    Assert.IsNotEmpty(noteString);
                    break;
                case "export":
                    context.Verify(
                        c =>
                            c.PostAsync(
                                It.Is<IMessageActivity>(a => a.AttachmentLayout == AttachmentLayoutTypes.Carousel),
                                It.IsAny<CancellationToken>()), Times.Once);
                    break;
            }
        }
    }
}