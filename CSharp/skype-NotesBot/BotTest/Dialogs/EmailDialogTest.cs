using System;
using System.Reflection;
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
    public class EmailDialogTest : DialogTestBase
    {
        [Test]
        [TestCase("contoso@microsoft.com", 0, "Your notes have been e-mailed.")]
        [TestCase("alice@wonderland", 0, "**Tries remaining: 2**\r\n\nPlease provide a valid email address.")]
        [TestCase("alice@wonderland", 2,
            "**Try exporting again.**\r\n\nSorry, you exceed the maximum number of tries to provide the email.")]
        public async Task CheckOnHandleEmailInputMethodWithInValidEmail(string message, int inputCount, string expected)
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
            var emailDialog = new EmailDialog();

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
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);
            context.Object.UserData.SetValue(BotConstants.EmailInputCountKey, inputCount);
            context.Object.UserData.SetValue(BotConstants.UserKey, user);
            context.Object.UserData.SetValue(BotConstants.AllNotesAsString, "test note");

            var methodInfo = emailDialog.GetType()
                .GetMethod(
                    "OnMessageReceivedAsync", BindingFlags.Static |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance);

            if (methodInfo == null)
            {
                Assert.Fail("Could not find OnMessageReceivedAsync method");
            }

            // Act
            methodInfo.Invoke(typeof(EmailDialog),
                new object[] {context.Object, TestHelper.CreateAwaitableMessage(message)});

            // Assert
            context.Verify(
                c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}