// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder
{
    public static class TeamsActivityExtensions
    {
        public static Activity CreateReply(this IActivity activity, string text = null, string locale = null)
        {
            return ((Activity)activity).CreateReply(text, locale);
        }
    }
}
