using System;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models
{
    /// <summary>
    /// Overload for messages sent via ChannelData
    /// </summary>
    public class SlackPostMessageFromChannelData : SlackPostMessage
    {
    }

    /// <summary>
    /// A message to post to Slack
    /// </summary>
    public class SlackPostMessage
    {
        // required
        public string channel;
        public string text;

        // optional
        [JsonConverter(typeof(BlockArrayConverter))]
        public ISlackBlock[] blocks;
        public string parse;
        public bool? link_names;
        public SlackPostAttachment[] attachments;
        public bool? unfurl_links;
        public bool? unfurl_media;
        public string username;
        public bool? as_user;
        public string icon_url;
        public string icon_emoji;
        public bool? mrkdwn;

        // threading (optional)
        public string thread_ts;
        public bool reply_broadcast;

        // orginal_message from Action only
        public string type;
        public string sub_type;
        public string bot_id;
        public string ts;

        // Action Only
        /// <summary>
        /// either "in_channel" or "ephemeral"
        /// </summary>
        public string response_type;
        public bool? replace_original;
        public bool? delete_original;

        // channeldata only - if a bot sets this to true we'll render buttons in a menu
        public bool? render_buttons_as_menu;

        public object Redact()
        {
            return new SlackPostMessage
            {
                channel = channel,
                thread_ts = thread_ts,
                reply_broadcast = reply_broadcast,
                type = type,
                sub_type = sub_type,
                bot_id = bot_id,
                ts = ts,
                response_type = response_type,
                replace_original = replace_original,
                delete_original = delete_original,
                render_buttons_as_menu = render_buttons_as_menu
            };
        }
    }

    public class SlackPostAttachment
    {
        [JsonConverter(typeof(BlockArrayConverter))]
        public ISlackBlock[] blocks { get; set; }
        public string fallback { get; set; }
        public string color { get; set; }
        public string pretext { get; set; }
        public string author_name { get; set; }
        public string author_link { get; set; }
        public string author_icon { get; set; }
        public string title { get; set; }
        public string title_link { get; set; }
        public string text { get; set; }
        public SlackField[] fields { get; set; }
        public string image_url { get; set; }
        public string thumb_url { get; set; }
        public string footer { get; set; }
        public string footer_icon { get; set; }
        public int ts { get; set; }
        /// <summary>
        /// Valid values for mrkdwn_in are: ["pretext", "text", "fields"]. Setting "fields" will enable markup formatting for the value of each field.
        /// </summary>
        public string[] mrkdwn_in { get; set; }
        public string callback_id { get; set; }
        public SlackAction[] actions { get; set; }
        public string attachment_type { get; set; }
    }

    public class SlackField
    {
        public string title { get; set; }
        public string value { get; set; }
        [JsonProperty(PropertyName = "short")]
        public bool _short { get; set; }
    }

    public class SlackAction
    {
        public string name { get; set; }
        public string text { get; set; }
        public string style { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string url { get; set; }
        public string data_source { get; set; }
        public int min_query_length { get; set; }
        public Option[] selected_options { get; set; }
        public Confirmation confirm { get; set; }
        public Option[] options { get; set; }
        public OptionGroup[] option_groups { get; set; }
    }

    public class OptionGroup
    {
        public string text { get; set; }
        public Option[] options { get; set; }
    }

    public class Option
    {
        public string value { get; set; }  // 2000 char max (https://api.slack.com/docs/interactive-message-field-guide#option_groups)
        public string text { get; set; }   // 30 char max 
        public string description { get; set; }  // 30 char max 
    }

    public class Confirmation
    {
        public string title { get; set; }
        public string text { get; set; }
        public string ok_text { get; set; }
        public string dismiss_text { get; set; }
    }
}