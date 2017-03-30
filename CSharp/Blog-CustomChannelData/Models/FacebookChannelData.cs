namespace Azure_Bot_Generic_CSharp.Models
{
    using Newtonsoft.Json;

    public class FacebookChannelData
    {
        [JsonProperty("attachment")]
        public FacebookAttachment Attachment
        {
            get;
            internal set;
        }
    }
}