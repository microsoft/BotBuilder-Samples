// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Configuration;
using System.Web.Http;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;

namespace AspNetWebApi_QnA_Bot
{
    public class BotConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapBotFramework(botConfig =>
            {
                botConfig
                    .UseMicrosoftApplicationIdentity(ConfigurationManager.AppSettings["BotFramework.MicrosoftApplicationId"], ConfigurationManager.AppSettings["BotFramework.MicrosoftApplicationPassword"]);
            });
        }
    }
}
