// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;

/// <summary>A <see cref="BotState"/> for managing bot state for "bot jobs".</summary>
/// <remarks>Independent from both <see cref="UserState"/> and <see cref="ConversationState"/> because
/// the process of running the jobs and notifying the user interacts with the
/// bot as a distinct user on a separate conversation.</remarks>
public class JobState : BotState
{
    /// <summary>The key used to cache the state information in the turn context.</summary>
    private const string StorageKey = "ProactiveBot.JobState";

    /// <summary>
    /// Initializes a new instance of the <see cref="JobState"/> class.</summary>
    /// <param name="storage">The storage provider to use.</param>
    public JobState(IStorage storage)
        : base(storage, StorageKey)
    {
    }

    /// <summary>Gets the storage key for caching state information.</summary>
    /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
    /// for processing this conversation turn.</param>
    /// <returns>The storage key.</returns>
    protected override string GetStorageKey(ITurnContext turnContext) => StorageKey;
}