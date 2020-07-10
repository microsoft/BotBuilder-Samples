// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TaskModuleFactorySample.Dialogs;
using TaskModuleFactorySample.Tests.Utterances;

namespace TaskModuleFactorySample.Tests
{
    [TestClass]
    public class SkillModeTests : SkillTestBase
    {
        [TestMethod]
        public async Task Test_Sample_Action()
        {
            await GetSkillTestFlow()
               .Send(new Activity(type: ActivityTypes.Event, name: "SampleAction"))
               .AssertReplyOneOf(GetTemplates("NamePromptText"))
               .Send(SampleDialogUtterances.NamePromptResponse)
               .AssertReplyOneOf(GetTemplates("HaveNameMessageText", new { Name = SampleDialogUtterances.NamePromptResponse }))
               .AssertReply((activity) =>
               {
                   var a = (Activity)activity;
                   Assert.AreEqual(ActivityTypes.EndOfConversation, a.Type);
                   Assert.AreEqual(typeof(SampleActionOutput), a.Value.GetType());
               })
               .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Sample_Action_w_Input()
        {
            var actionInput = new SampleActionInput() { Name = "test" };

            await GetSkillTestFlow()
               .Send(new Activity(type: ActivityTypes.Event, name: "SampleAction", value: JObject.FromObject(actionInput)))
               .AssertReplyOneOf(GetTemplates("HaveNameMessageText", new { actionInput.Name }))
               .AssertReply((activity) =>
               {
                   var a = (Activity)activity;
                   Assert.AreEqual(ActivityTypes.EndOfConversation, a.Type);
                   Assert.AreEqual(typeof(SampleActionOutput), a.Value.GetType());
               })
               .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Sample_Dialog()
        {
            await GetSkillTestFlow()
               .Send(SampleDialogUtterances.Trigger)
               .AssertReplyOneOf(GetTemplates("NamePromptText"))
               .Send(SampleDialogUtterances.NamePromptResponse)
               .AssertReplyOneOf(GetTemplates("HaveNameMessageText", new { Name = SampleDialogUtterances.NamePromptResponse }))
               .AssertReply((activity) => { Assert.AreEqual(ActivityTypes.EndOfConversation, activity.Type); })
               .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_End_Of_Conversation()
        {
            await GetSkillTestFlow()
                .Send(GeneralUtterances.None)
                .AssertReplyOneOf(GetTemplates("UnsupportedText"))
                .AssertReply((activity) => { Assert.AreEqual(ActivityTypes.EndOfConversation, activity.Type); })
                .StartTestAsync();
        }
    }
}
