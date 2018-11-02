// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This custom BotAdapter supports scenarios that only Send Activities. Update and Delete Activity
    /// are not supported.
    /// Rather than sending the outbound Activities directly as the BotFrameworkAdapter does this class
    /// buffers them in a list. The list is exposed as a public property.
    /// </summary>
    public class DialogHostAdapter : BotAdapter
    {
        private List<Activity> _response = new List<Activity>();

        public IEnumerable<Activity> Activities => _response;

        public override Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, Activity[] activities, CancellationToken cancellationToken)
        {
            foreach (var activity in activities)
            {
                _response.Add(activity);
            }

            return Task.FromResult(new ResourceResponse[0]);
        }

        #region Not Implemented
        public override Task DeleteActivityAsync(ITurnContext turnContext, ConversationReference reference, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<ResourceResponse> UpdateActivityAsync(ITurnContext turnContext, Activity activity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
