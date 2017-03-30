namespace Azure_Bot_Generic_CSharp.Models
{
    using Newtonsoft.Json;

    public class FacebookGenericTemplate
    {
        public FacebookGenericTemplate()
        {
            this.TemplateType = "generic";
        }

        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("elements")]
        public object[] Elements { get; set; }
    }
}