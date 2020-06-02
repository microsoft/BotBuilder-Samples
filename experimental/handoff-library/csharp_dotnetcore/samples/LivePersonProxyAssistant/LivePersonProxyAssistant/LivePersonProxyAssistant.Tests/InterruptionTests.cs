// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LivePersonProxyAssistant.Tests.Utterances;

namespace LivePersonProxyAssistant.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class InterruptionTests : BotTestBase
    {
        [TestMethod]
        public async Task Test_Help_Interruption()
        {
            var allFirstPromptVariations = AllResponsesTemplates.ExpandTemplate("FirstPromptMessage");

            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(allFirstPromptVariations.Cast<string>().ToArray())
                .Send(GeneralUtterances.Help)
                .AssertReply(activity => Assert.AreEqual(1, activity.AsMessageActivity().Attachments.Count))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Help_Interruption_In_Dialog()
        {
            var allNamePromptVariations = AllResponsesTemplates.ExpandTemplate("NamePrompt");

            await GetTestFlow(includeUserProfile: false)
                .Send(string.Empty)
                .AssertReplyOneOf(allNamePromptVariations.Cast<string>().ToArray())
                .Send(GeneralUtterances.Help)
                .AssertReply(activity => Assert.AreEqual(1, activity.AsMessageActivity().Attachments.Count))
                .AssertReplyOneOf(allNamePromptVariations.Cast<string>().ToArray())
                .StartTestAsync();
        }

        [TestMethod]
        [Ignore("the LG template 'UnsupportedMessage' has randomly generated response which makes this test unreliable")]
        public async Task Test_Cancel_Interruption()
        {
            var allFirstPromptVariations = AllResponsesTemplates.ExpandTemplate("FirstPromptMessage");
            var allResponseVariations = AllResponsesTemplates.ExpandTemplate("CancelledMessage", TestUserProfileState);

            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(allFirstPromptVariations.Cast<string>().ToArray())
                .Send(GeneralUtterances.Cancel)
                .AssertReplyOneOf(allResponseVariations.Cast<string>().ToArray())
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Repeat_Interruption()
        {
            var allNamePromptVariations = AllResponsesTemplates.ExpandTemplate("NamePrompt");

            await GetTestFlow(includeUserProfile: false)
                .Send(string.Empty)
                .AssertReplyOneOf(allNamePromptVariations.Cast<string>().ToArray())
                .Send(GeneralUtterances.Repeat)
                .AssertReplyOneOf(allNamePromptVariations.Cast<string>().ToArray())
                .StartTestAsync();
        }
    }
}
