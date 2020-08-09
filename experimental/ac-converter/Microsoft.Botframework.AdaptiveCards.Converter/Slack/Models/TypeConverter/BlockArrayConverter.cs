using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class BlockArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ISlackBlock));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<ISlackBlock>();
            var array = JArray.Load(reader);
            foreach (JObject jsonObject in array)
            {
                ISlackBlock SlackBlock = null;
                var blockType = jsonObject.Value<string>("type");
                switch (blockType)
                {
                    case "actions":
                        SlackBlock = serializer.Deserialize<ActionsBlock>(jsonObject.CreateReader());
                        break;
                    case "context":
                        SlackBlock = serializer.Deserialize<ContextBlock>(jsonObject.CreateReader());
                        break;
                    case "divider":
                        SlackBlock = serializer.Deserialize<DividerBlock>(jsonObject.CreateReader());
                        break;
                    case "image":
                        SlackBlock = serializer.Deserialize<ImageBlock>(jsonObject.CreateReader());
                        break;
                    case "input":
                        SlackBlock = serializer.Deserialize<InputBlock>(jsonObject.CreateReader());
                        break;
                    case "section":
                        SlackBlock = serializer.Deserialize<SectionBlock>(jsonObject.CreateReader());
                        break;
                    default:
                        break;
                }

                result.Add(SlackBlock);
            }

            return result.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}