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
    /// The interface allows callers to track the completion of the handoff request.
    /// </summary>
    public interface IHandoffRequest
    {
        Task<bool> IsCompletedAsync();
    }
}
