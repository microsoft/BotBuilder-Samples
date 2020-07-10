// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TaskModuleFactorySample.Extensions.Teams.TaskModule
{
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;

    /// <summary>
    /// TaskInfo class
    /// </summary>
    public class TaskInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("card")]
        public Attachment Card { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("height")]
        public object Height { get; set; }

        [JsonProperty("width")]
        public object Width { get; set; }

        [JsonProperty("fallbackUrl")]
        public string FallbackUrl { get; set; }

        [JsonProperty("completionBotId")]
        public string CompletionBotId { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
