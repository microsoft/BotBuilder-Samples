using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Bot;
using Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace BotTest.Dialogs
{
    [TestClass]
    public sealed class HelpDialogTest : DialogTestBase
    {
        [Test]
        [TestCase(BotConstants.Help, true, "")]
        [TestCase(BotConstants.HelpNote, false, BotConstants.NoteHelpText)]
        [TestCase(BotConstants.HelpShow, false, BotConstants.ShowHelpText)]
        [TestCase(BotConstants.HelpDelete, false, BotConstants.DeleteHelpText)]
        [TestCase(BotConstants.HelpExport, false, BotConstants.ExportHelpText)]
        public async Task CheckStartAsyncMethod(string helpType, bool isGenericHelp, string expected)
        {
            // Arrange
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
            context.Setup(c => c.UserData).Returns(() => dataBag);
            context.Object.UserData.SetValue(BotConstants.HelpTypeKey, helpType);
            context.Object.UserData.SetValue(BotConstants.IsGenericHelpKey, isGenericHelp);
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);

            var helpDialog = new HelpDialog();

            // Act
            await helpDialog.StartAsync(context.Object);

            // Assert
            if (isGenericHelp)
            {
                context.Verify(
                    c => c.PostAsync(It.Is<IMessageActivity>(a => a.AttachmentLayout == AttachmentLayoutTypes.Carousel),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }
            else
            {
                context.Verify(
                    c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }
        }

        [Test]
        [TestCase(BotConstants.HelpNote, BotConstants.NoteHelpText)]
        [TestCase(BotConstants.HelpShow, BotConstants.ShowHelpText)]
        [TestCase(BotConstants.HelpDelete, BotConstants.DeleteHelpText)]
        [TestCase(BotConstants.HelpExport, BotConstants.ExportHelpText)]
        public void CheckMessageReceivedAsync(string helpType, string expected)
        {
            // Arrange
            var context = new Mock<IDialogContext>();
            context.Setup(c => c.Activity).Returns(() => new Activity());
            context.Setup(c => c.MakeMessage()).Returns(() => new Activity() as IMessageActivity);
            var helpDialog = new HelpDialog();
            var methodInfo = helpDialog.GetType()
                .GetMethod(
                    "OnMessageReceivedAsync", BindingFlags.Static |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance);

            if (methodInfo == null)
            {
                Assert.Fail("Could not find OnMessageReceivedAsync method");
            }

            // Act
            methodInfo.Invoke(typeof(HelpDialog),
                new object[] {context.Object, TestHelper.CreateAwaitableMessage(helpType)});

            // Assert
            context.Verify(
                c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Contains(expected)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}