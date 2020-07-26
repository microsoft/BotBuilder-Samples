// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
using Microsoft.Bot.Schema;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ImmediateAcceptBot.BackgroundQueue
{
    /// <summary>
    /// Interface for a class used to transfer an AcctivityWithClaims to the <see cref="HostedActivityService"/>.
    /// </summary>
    public interface IActivityTaskQueue
    {
        /// <summary>
        /// Enqueue an Activity, with Claims, to be processed on a background thread.
        /// 
        /// NOTE: It is assumed these claims have been authenticated via JwtTokenValidation.AuthenticateRequest 
        /// before enqueueing.
        /// </summary>
        /// <param name="claimsIdentity">Authenticated <see cref="ClaimsIdentity"/> used to process the 
        /// activity.</param>
        /// <param name="activity"><see cref="Activity"/> to be processed.</param>
        void QueueBackgroundActivity(ClaimsIdentity claimsIdentity, Activity activity);

        /// <summary>
        /// Wait for a signal of an enqueued Activity with Claims to be processed.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken used to cancel the wait.</param>
        /// <returns>An ActivityWithClaims to be processed.
        /// 
        /// NOTE: It is assumed these claims have already been authenticated.
        /// </returns>
        Task<ActivityWithClaims> WaitForActivityAsync(CancellationToken cancellationToken);
    }
}
