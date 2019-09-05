// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

using AdaptiveCards;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
namespace TeamsTasksBot.Helper
{
    public static class ApplicationSettings
    {
        public static string BaseUrl { get; set; }

        public static string MicrosoftAppId { get; set; }

        //static ApplicationSettings()
        //{
        //    BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        //    MicrosoftAppId = ConfigurationManager.AppSettings["MicrosoftAppId"];

        //}
    }
}