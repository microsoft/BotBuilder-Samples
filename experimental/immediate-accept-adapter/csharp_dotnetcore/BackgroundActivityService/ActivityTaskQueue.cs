// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
using Microsoft.Bot.Schema;
using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ImmediateAcceptBot.BackgroundQueue
{
    /// <summary>
    /// Singleton queue, used to transfer an AcctivityWithClaims to the <see cref="HostedActivityService"/>.
    /// </summary>
    public class ActivityTaskQueue : IActivityTaskQueue
    {
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private ConcurrentQueue<ActivityWithClaims> _activities = new ConcurrentQueue<ActivityWithClaims>();

        /// <summary>
        /// Enqueue an Activity, with Claims, to be processed on a background thread.
        /// 
        /// NOTE: It is assumed these claims have been authenticated via JwtTokenValidation.AuthenticateRequest 
        /// before enqueueing.
        /// </summary>
        /// <param name="claimsIdentity">Authenticated <see cref="ClaimsIdentity"/> used to process the 
        /// activity.</param>
        /// <param name="activity"><see cref="Activity"/> to be processed.</param>
        public void QueueBackgroundActivity(ClaimsIdentity claims, Activity activity)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            _activities.Enqueue(new ActivityWithClaims { ClaimsIdentity = claims, Activity = activity});
            _signal.Release();
        }

        /// <summary>
        /// Wait for a signal of an enqueued Activity with Claims to be processed.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken used to cancel the wait.</param>
        /// <returns>An ActivityWithClaims to be processed.
        /// 
        /// NOTE: It is assumed these claims have already been authenticated via JwtTokenValidation.AuthenticateRequest.
        /// </returns>
        public async Task<ActivityWithClaims> WaitForActivityAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            ActivityWithClaims dequeued;
            _activities.TryDequeue(out dequeued);

            return dequeued;
        }
    }
}
