// <copyright file="OperationDetails.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.AuditBot
{
    using System;

    /// <summary>
    /// Operation details of a team operation.
    /// </summary>
    public class OperationDetails
    {
        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the operation time.
        /// </summary>
        public DateTimeOffset OperationTime { get; set; }
    }
}
