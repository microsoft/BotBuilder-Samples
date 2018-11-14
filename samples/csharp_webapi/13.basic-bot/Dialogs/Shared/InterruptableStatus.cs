// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BasicBot
{
    /// <summary>
    /// Possible interruption status of interruptable dialogs.
    /// </summary>
    public enum InterruptionStatus
    {
        /// <summary>
        /// Indicates that the active dialog was interrupted and needs to resume.
        /// </summary>
        Interrupted,

        /// <summary>
        /// Indicates that there is a new dialog waiting and the active dialog needs to be shelved.
        /// </summary>
        Waiting,

        /// <summary>
        /// Indicates that no interruption action is required.
        /// </summary>
        NoAction,
    }
}
