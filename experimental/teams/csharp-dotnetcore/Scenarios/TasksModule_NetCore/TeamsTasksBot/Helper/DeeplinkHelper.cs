// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

using System.Web;

namespace TeamsTasksBot.Helper
{
    public static class DeeplinkHelper
    {
        public static string DeepLink { get; set; }
        public static string DeepLinkToAdaptiveCard { get; set; }

        static DeeplinkHelper()
        {
            DeepLink = string.Format("https://teams.microsoft.com/l/task/{0}?url={1}&height={2}&width={3}&title={4}&completionBotId={5}",
              ApplicationSettings.MicrosoftAppId,
              HttpUtility.UrlEncode(ApplicationSettings.BaseUrl + "/customForm"),
              TaskModuleUIConstants.CustomForm.Height,
              TaskModuleUIConstants.CustomForm.Width,
              HttpUtility.UrlEncode(TaskModuleUIConstants.CustomForm.Title),
              ApplicationSettings.MicrosoftAppId);
        }
    }
}