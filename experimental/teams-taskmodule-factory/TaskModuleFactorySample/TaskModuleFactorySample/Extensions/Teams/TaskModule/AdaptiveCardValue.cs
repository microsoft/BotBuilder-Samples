// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.Extensions.Teams.TaskModule
{
    using Newtonsoft.Json;

    /// <summary>
    /// DataContract on AdaptieCardValue
    /// </summary>
    public class AdaptiveCardValue<T>
    {
        [JsonProperty("msteams")]
        public object Type { get; set; } = JsonConvert.DeserializeObject("{\"type\": \"task/fetch\" }");

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
