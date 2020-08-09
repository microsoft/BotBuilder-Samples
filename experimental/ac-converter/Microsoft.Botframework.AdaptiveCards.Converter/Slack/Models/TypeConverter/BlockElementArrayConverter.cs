using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class BlockElementArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IBlockElement));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<IBlockElement>();
            var array = JArray.Load(reader);
            foreach (JObject jsonObject in array)
            {
                IBlockElement blockElement = null;
                var blockType = jsonObject.Value<string>("type");
                switch (blockType)
                {
                    case "button":
                        blockElement = serializer.Deserialize<Button>(jsonObject.CreateReader());
                        break;
                    case "channels_select":
                        blockElement = serializer.Deserialize<ChannelsSelect>(jsonObject.CreateReader());
                        break;
                    case "checkboxes":
                        blockElement = serializer.Deserialize<Checkboxes>(jsonObject.CreateReader());
                        break;
                    case "conversations_select":
                        blockElement = serializer.Deserialize<ConversationsSelect>(jsonObject.CreateReader());
                        break;
                    case "datepicker":
                        blockElement = serializer.Deserialize<DatePicker>(jsonObject.CreateReader());
                        break;
                    case "external_select":
                        blockElement = serializer.Deserialize<ExternalSelect>(jsonObject.CreateReader());
                        break;
                    case "image":
                        blockElement = serializer.Deserialize<Image>(jsonObject.CreateReader());
                        break;
                    case "multi_channels_select":
                        blockElement = serializer.Deserialize<MultiChannelsSelect>(jsonObject.CreateReader());
                        break;
                    case "multi_conversations_select":
                        blockElement = serializer.Deserialize<MultiConversationsSelect>(jsonObject.CreateReader());
                        break;
                    case "multi_external_select":
                        blockElement = serializer.Deserialize<MultiExternalSelect>(jsonObject.CreateReader());
                        break;
                    case "multi_static_select":
                        blockElement = serializer.Deserialize<MultiStaticSelect>(jsonObject.CreateReader());
                        break;
                    case "multi_users_select":
                        blockElement = serializer.Deserialize<MultiUsersSelect>(jsonObject.CreateReader());
                        break;
                    case "overflow":
                        blockElement = serializer.Deserialize<OverflowMenu>(jsonObject.CreateReader());
                        break;
                    case "plain_text_input":
                        blockElement = serializer.Deserialize<PlainTextInput>(jsonObject.CreateReader());
                        break;
                    case "radio_buttons":
                        blockElement = serializer.Deserialize<RadioButton>(jsonObject.CreateReader());
                        break;
                    case "static_select":
                        blockElement = serializer.Deserialize<StaticSelect>(jsonObject.CreateReader());
                        break;
                    case "users_select":
                        blockElement = serializer.Deserialize<UsersSelect>(jsonObject.CreateReader());
                        break;
                    default:
                        break;
                }

                result.Add(blockElement);
            }

            return result.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}