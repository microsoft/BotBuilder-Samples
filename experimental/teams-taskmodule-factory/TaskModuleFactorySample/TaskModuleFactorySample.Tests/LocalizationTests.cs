// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaskModuleFactorySample.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class LocalizationTests : SkillTestBase
    {
        [TestMethod]
        public async Task Test_Localization_Spanish()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("es-es");

            await GetTestFlow()
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(TemplateEngine.GenerateActivityForLocale("IntroMessage"))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Localization_German()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("de-de");

            await GetTestFlow()
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(TemplateEngine.GenerateActivityForLocale("IntroMessage"))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Localization_French()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-fr");

            await GetTestFlow()
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(TemplateEngine.GenerateActivityForLocale("IntroMessage"))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Localization_Italian()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("it-it");

            await GetTestFlow()
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(TemplateEngine.GenerateActivityForLocale("IntroMessage"))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Localization_Chinese()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("zh-cn");

            await GetTestFlow()
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(TemplateEngine.GenerateActivityForLocale("IntroMessage"))
                .StartTestAsync();
        }

        [TestMethod]
        public async Task Test_Defaulting_Localization()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-uk");
            await GetTestFlow()
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(TemplateEngine.GenerateActivityForLocale("IntroMessage"))
                .StartTestAsync();
        }
    }
}