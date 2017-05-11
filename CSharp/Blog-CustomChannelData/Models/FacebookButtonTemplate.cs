namespace Azure_Bot_Generic_CSharp.Models
{
    using Newtonsoft.Json;

    public class FacebookButtonTemplate
    {
        public FacebookButtonTemplate()
        {
            this.TemplateType = "button";
            this.Text = "This is default text.";
        }
        public FacebookButtonTemplate(string text)
        {
            this.TemplateType = "button";
            this.Text = text;
        }

        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("buttons")]
        public FacebookPostbackButton[] Buttons { get; set; }

        public override string ToString()
        {
            return $"template_type: {this.TemplateType}\ntext: {this.Text}\nbuttons:\n\t {this.Buttons}";
        }
    }
}