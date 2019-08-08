// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Handoff
{
    /// <summary>
    /// Extends ITurnContext to add InitiateHandoffAsync.
    /// </summary>
    public static class TurnContextExtender
    {
        /// <summary>
        /// Initiate handoff to human agent.
        /// </summary>
        /// <param name="turnContext">Turn context.</param>
        /// <param name="activities">Transcript of the activities that took place so far.</param>
        /// <param name="handoffContext">Additional channel-specific content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IHandoffRequest.</returns>
        public static Task<HandoffRequest> InitiateHandoffAsync(this ITurnContext turnContext, IActivity[] activities, object handoffContext, CancellationToken cancellationToken = default)
        {
            if (activities == null)
            {
                throw new ArgumentNullException(nameof(activities));
            }

            if (handoffContext == null)
            {
                throw new ArgumentNullException(nameof(handoffContext));
            }

            var adapter = turnContext.Adapter;
            if (adapter is IHandoffAdapter handoffAdapter)
            {
                var conversationReference = turnContext.Activity.GetConversationReference();
                var bufferedActivities = activities.Select(a => a.ApplyConversationReference(conversationReference)).ToArray();
                return handoffAdapter.InitiateHandoffAsync(turnContext, bufferedActivities.ToArray(), handoffContext, cancellationToken);
            }

            throw new NotSupportedException("Must use an adapter that supports handoff");
        }
    }
}
