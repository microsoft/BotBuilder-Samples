// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Configuration;
using System.Web.Http;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;

namespace AspNetWebApi_QnA_Bot
{
    /// <summary>
    /// Performs the BotConfiguration-specific configuration during Asp.Net start.
    /// </summary>
    public class BotConfig
    {
        /// <summary>
        /// Register the bot framwork with Asp.net.
        /// </summary>
        /// <param name="config">Represents the configuration of the HttpServer.</param>
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
