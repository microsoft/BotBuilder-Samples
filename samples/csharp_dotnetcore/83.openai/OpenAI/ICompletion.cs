// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public interface ICompletion
    {
        Task<string> GenerateCompletionAsync(ITurnContext<IMessageActivity> turnContext);
    }
}
