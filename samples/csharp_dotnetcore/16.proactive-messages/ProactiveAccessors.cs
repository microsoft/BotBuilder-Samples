// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See <see cref="ProactiveBot"/> constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class ProactiveAccessors
    {
        /// <summary>
        /// A unique ID to use for this property accessor.
        /// </summary>
        public const string JobLogDataName = "ProactiveBot.JobLogAccessor";

        public ProactiveAccessors(JobState jobState)
        {
            JobState = jobState;
        }

        /// <summary>
        /// Gets or sets the state property accessor for the job log.
        /// </summary>
        /// <value>
        /// "Running" jobs (represented by <see cref="JobLog.JobData"/>).
        /// </value>
        public IStatePropertyAccessor<JobLog> JobLogData { get; set; }

        /// <summary>
        /// Gets the JobState object.
        /// </summary>
        /// <value>
        /// User and Conversation independent state object.
        /// </value>
        public JobState JobState { get; }
    }
}
