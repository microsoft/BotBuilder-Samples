// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class SlotDetails
    {
        public SlotDetails(string name, string promptId, string prompt = null, string retryPrompt = null)
            : this(name, promptId, new PromptOptions
            {
                Prompt = MessageFactory.Text(prompt),
                RetryPrompt = MessageFactory.Text(retryPrompt),
            })
        {
        }

        public SlotDetails(string name, string promptId, PromptOptions options)
        {
            Name = name;
            PromptId = promptId;
            Options = options;
        }

        public string Name { get; set; }

        public string PromptId { get; set; }

        public PromptOptions Options { get; set; }
    }
}
