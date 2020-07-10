// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.Extensions.Teams.TaskModule
{
    using Newtonsoft.Json;

    /// <summary>
    /// TaskEnvelope class with TaskProperty.
    /// </summary>
    public class TaskEnvelope : ITeamsInvokeEnvelope
    {
        [JsonProperty("task")]
        public TaskProperty Task { get; set; }
    }
}
