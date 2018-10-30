// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ScaleoutBot
{
    public interface IStore
    {
        Task<(JObject content, string eTag)> LoadAsync(string key);

        Task<bool> SaveAsync(string key, JObject content, string eTag);
    }
}
