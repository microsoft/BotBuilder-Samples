// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A list of SlotDetails defines the behavior of our SlotFillingDialog.
    /// This class represents a description of a single 'slot'. It contains the name of the property we want to gather
    /// and the id of the corresponding dialog that should be used to gather that property. The id is that used when the
    /// dialog was added to the current DialogSet. Typically this id is that of a prompt but it could also be the id of
    /// another slot dialog.
    /// </summary>
    public class SlotDetails
    {
        public SlotDetails(string name, string dialogId, string prompt = null, string retryPrompt = null)
            : this(name, dialogId, new PromptOptions
            {
                Prompt = MessageFactory.Text(prompt),
                RetryPrompt = MessageFactory.Text(retryPrompt),
            })
        {
        }

        public SlotDetails(string name, string dialogId, PromptOptions options)
        {
            Name = name;
            DialogId = dialogId;
            Options = options;
        }

        public string Name { get; set; }

        public string DialogId { get; set; }

        public PromptOptions Options { get; set; }
    }
}
