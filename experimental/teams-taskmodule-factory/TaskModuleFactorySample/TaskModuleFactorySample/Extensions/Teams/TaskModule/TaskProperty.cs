// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.Extensions.Teams.TaskModule
{
    using Newtonsoft.Json;

    /// <summary>
    /// TaskProperty
    /// </summary>
    public class TaskProperty
    {
        [JsonProperty("value")]
        public TaskInfo TaskInfo { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
