// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    using System.Collections.Generic;

    /// <summary>
    /// Team operation history.
    /// </summary>
    public class TeamOperationHistory
    {
        /// <summary>
        /// Gets or sets the member operations. Operation is a tuple of ObjectId, Operation and Time.
        /// </summary>
        public List<OperationDetails> MemberOperations { get; set; } = new List<OperationDetails>();
    }
}
