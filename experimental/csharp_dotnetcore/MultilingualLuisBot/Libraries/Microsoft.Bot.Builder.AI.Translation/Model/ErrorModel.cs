// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.AI.Translation.Model
{
    /// <summary>
    /// Standard error from Translator API v3.
    /// </summary>
    internal class ErrorModel
    {
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }
}
