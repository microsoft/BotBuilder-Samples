using System;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    public class BlockElementConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IBlockElement));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Button))
            {
                return serializer.Deserialize<Button>(reader);
            }

            if (objectType == typeof(ChannelsSelect))
            {
                return serializer.Deserialize<ChannelsSelect>(reader);
            }

            if (objectType == typeof(Checkboxes))
            {
                return serializer.Deserialize<Checkboxes>(reader);
            }
           
            if (objectType == typeof(ConversationsSelect))
            {
                return serializer.Deserialize<ConversationsSelect>(reader);
            }

            if (objectType == typeof(DatePicker))
            {
                return serializer.Deserialize<DatePicker>(reader);
            }

            if (objectType == typeof(ExternalSelect))
            {
                return serializer.Deserialize<ExternalSelect>(reader);
            }

            if (objectType == typeof(Image))
            {
                return serializer.Deserialize<Image>(reader);
            }

            if (objectType == typeof(MultiChannelsSelect))
            {
                return serializer.Deserialize<MultiChannelsSelect>(reader);
            }

            if (objectType == typeof(MultiConversationsSelect))
            {
                return serializer.Deserialize<MultiConversationsSelect>(reader);
            }

            if (objectType == typeof(MultiExternalSelect))
            {
                return serializer.Deserialize<MultiExternalSelect>(reader);
            }

            if (objectType == typeof(MultiStaticSelect))
            {
                return serializer.Deserialize<MultiStaticSelect>(reader);
            }

            if (objectType == typeof(MultiUsersSelect))
            {
                return serializer.Deserialize<MultiUsersSelect>(reader);
            }

            if (objectType == typeof(OverflowMenu))
            {
                return serializer.Deserialize<OverflowMenu>(reader);
            }

            if (objectType == typeof(PlainTextInput))
            {
                return serializer.Deserialize<PlainTextInput>(reader);
            }

            if (objectType == typeof(RadioButton))
            {
                return serializer.Deserialize<RadioButton>(reader);
            }

            if (objectType == typeof(StaticSelect))
            {
                return serializer.Deserialize<StaticSelect>(reader);
            }

            if (objectType == typeof(UsersSelect))
            {
                return serializer.Deserialize<UsersSelect>(reader);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}