// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImmediateAcceptBot.BackgroundQueue
{
    /// <summary>
    /// Interface for a class used to transfer a workitem to the <see cref="HostedTaskService"/>.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Enqueue a work item to be processed on a background thread.
        /// </summary>
        /// <param name="workItem">The work item to be enqueued for execution, is defined as
        /// a function which takes a cancellation token.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Wait for a signal of an enqueued work item to be processed.
        /// </summary>
        /// <param name="cancellationToken">CancellationToken used to cancel the wait.</param>
        /// <returns>A function taking a cacnellation token, which needs to be processed.
        /// </returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
