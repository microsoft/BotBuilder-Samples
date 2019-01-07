// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// An ETag aware store definition.
    /// The interface is defined in terms of JObject to move serialization out of the storage layer
    /// while still indicating it is JSON, a fact the store may choose to make use of.
    /// </summary>
    public interface IStore
    {
        Task<(JObject content, string etag)> LoadAsync(string key);

        Task<bool> SaveAsync(string key, JObject content, string etag);
    }
}
