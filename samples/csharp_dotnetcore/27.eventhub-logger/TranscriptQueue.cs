// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// An in-memory queue for transcript backed by a concurrent queue.
    /// </summary>
    public class TranscriptQueue : ITranscriptQueue
    {
        private readonly ConcurrentQueue<string> _cq = new ConcurrentQueue<string>();

        public void Enqueue(string value) => _cq.Enqueue(value);

        public (bool, string) TryDequeue()
        {
            var found = _cq.TryDequeue(out var result);
            return (found, result);
        }

        public (bool, string) TryPeek()
        {
            var found = _cq.TryPeek(out var result);
            return (found, result);
        }
    }
}
