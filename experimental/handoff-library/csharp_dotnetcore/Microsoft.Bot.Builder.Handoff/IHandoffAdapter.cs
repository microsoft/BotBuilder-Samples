// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Handoff
{
    /// <summary>
    /// Adapters that support handoff shall implement this interface.
    /// </summary>
    public interface IHandoffAdapter
    {
        Task<HandoffRequest> InitiateHandoffAsync(ITurnContext turnContext, Activity[] activities, object handoffContext, CancellationToken cancellationToken = default);
    }
}
