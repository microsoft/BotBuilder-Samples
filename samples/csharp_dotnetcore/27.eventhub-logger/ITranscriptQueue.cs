// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A queue bridges a synchronous telemetry client (e.g., TrackEvent) and
    /// an asynchronous background processing (e.g., a background hosted service).
    /// </summary>
    public interface ITranscriptQueue
    {
        void Enqueue(string eventData);
        (bool, string) TryDequeue();
        (bool, string) TryPeek();
    }
}
