// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A thread safe implementation of the IStore abstraction intended for testing.
    /// </summary>
    public class MemoryStore : IStore
    {
        private IDictionary<string, (JObject, string)> _store = new Dictionary<string, (JObject, string)>();
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<(JObject content, string etag)> LoadAsync(string key)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (_store.TryGetValue(key, out ValueTuple<JObject, string> value))
                {
                    return value;
                }

                return new ValueTuple<JObject, string>(null, null);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> SaveAsync(string key, JObject content, string eTag)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (eTag != null && _store.TryGetValue(key, out ValueTuple<JObject, string> value))
                {
                    if (eTag != value.Item2)
                    {
                        return false;
                    }
                }

                _store[key] = (content, Guid.NewGuid().ToString());
                return true;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
