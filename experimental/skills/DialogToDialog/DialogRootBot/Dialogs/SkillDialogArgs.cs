// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Dialogs
{
    /// <summary>
    /// A class with dialog arguments for a <see cref="SkillDialog"/>.
    /// </summary>
    public class SkillDialogArgs
    {
        /// <summary>
        /// Gets or sets the ID of the skill to invoke.
        /// </summary>
        public string SkillId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ActivityTypes"/> to send to the skill.
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// Gets or sets the name of the event or invoke activity to send to the skill (this value is ignored for other types of activities)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value property for the activity to send to the skill.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the text property for the <see cref="ActivityTypes.Message"/> to send to the skill (ignored for other types of activities).
        /// </summary>
        public string Text { get; set; }
    }
}
