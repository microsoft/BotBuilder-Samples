// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Activity = Microsoft.Bot.Schema.Activity;

namespace Microsoft.BotBuilderSamples
{
    internal static class ActivityHelper
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        public static string ToJson(IActivity activity) => JsonConvert.SerializeObject(activity, JsonSettings);

        public static Activity Clone(IActivity activity) => JsonConvert.DeserializeObject<Activity>(ToJson(activity));
    }
}
