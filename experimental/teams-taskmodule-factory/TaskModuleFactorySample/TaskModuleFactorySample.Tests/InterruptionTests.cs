// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskModuleFactorySample.Tests.Utterances;

namespace TaskModuleFactorySample.Tests
{
    [TestClass]
    public class InterruptionTests : SkillTestBase
    {
        [TestMethod]
        public async Task Test_Help_Interruption()
        {
            await GetTestFlow()
                .Send(SampleDialogUtterances.Trigger)
                .AssertReplyOneOf(GetTemplates("FirstPromptText"))
                .Send(SampleDialogUtterances.Trigger)
                .AssertReplyOneOf(GetTemplates("NamePromptText"))
                .Send(GeneralUtterances.Help)
                .AssertReply(activity => Assert.AreEqual(1, activity.AsMessageActivity().Attachments.Count))
                .AssertReplyOneOf(GetTemplates("NamePromptText"))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Cancel_Interruption()
        {
            await GetTestFlow()
                .Send(SampleDialogUtterances.Trigger)
                .AssertReplyOneOf(GetTemplates("FirstPromptText"))
                .Send(SampleDialogUtterances.Trigger)
                .AssertReplyOneOf(GetTemplates("NamePromptText"))
                .Send(GeneralUtterances.Cancel)
                .AssertReplyOneOf(GetTemplates("CancelledText"))
                .StartTestAsync();
        }
    }
}
