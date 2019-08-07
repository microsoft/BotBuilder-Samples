// <copyright file="TaskModuleAction.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using AdaptiveCards;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Adapter class to represent BotBuilder card action as adaptive card action (in type of Action.Submit).
    /// </summary>
    public class TaskModuleAction : CardAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskModuleAction"/> class.
        /// </summary>
        /// <param name="title">Button title</param>
        /// <param name="value">Free hidden value binding with button. The value will be sent out with "task/fetch" invoke event.</param>
        public TaskModuleAction(string title, object value)
            : base("invoke", title)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            JToken data;
            if (value is string)
            {
                data = JObject.Parse(value as string);
            }
            else
            {
                data = JObject.FromObject(value, JsonSerializer.Create(serializerSettings));
            }

            data["type"] = "task/fetch";
            this.Value = data.ToString();
        }
    }
}
