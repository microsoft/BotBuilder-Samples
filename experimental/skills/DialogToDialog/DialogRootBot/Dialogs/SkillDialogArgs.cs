// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples.DialogRootBot.Dialogs
{
    /// <summary>
    /// A class with dialog arguments for a <see cref="SkillDialog"/>.
    /// </summary>
    public class SkillDialogArgs
    {
        public string SkillId { get; set; }

        public string EventName { get; set; }

        public object Value { get; set; }
    }
}
