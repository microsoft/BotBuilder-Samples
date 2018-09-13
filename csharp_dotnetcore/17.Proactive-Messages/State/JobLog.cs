// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>Contains a dictionary of job data, indexed by job number.</summary>
    /// <remarks>The JobLog class tracks all the outstanding jobs.  Each job is
    /// identified by a unique key.</remarks>
    public class JobLog : Dictionary<long, JobLog.JobData>
    {
        /// <summary>Describes the state of a job.</summary>
        public class JobData
        {
            /// <summary>Gets or sets the time-stamp for the job.</summary>
            /// <value>
            /// The time-stamp for the job when the job needs to fire.
            /// </value>
            public long TimeStamp { get; set; } = 0;

            /// <summary>Gets or sets a value indicating whether indicates whether the job has completed.</summary>
            /// <value>
            /// A value indicating whether indicates whether the job has completed.
            /// </value>
            public bool Completed { get; set; } = false;

            /// <summary>
            /// Gets or sets the conversation reference to which to send status updates.
            /// </summary>
            /// <value>
            /// The conversation reference to which to send status updates.
            /// </value>
            public ConversationReference Conversation { get; set; }
        }
    }
}