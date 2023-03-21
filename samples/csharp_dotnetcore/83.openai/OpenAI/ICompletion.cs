// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Microsoft.BotBuilderSamples
{
    public interface ICompletion
    {
        Task<string> GenerateCompletionAsync(IEnumerable<ChatMessage> history);
    }
}
