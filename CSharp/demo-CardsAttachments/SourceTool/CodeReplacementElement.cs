namespace SourceTool
{
    using System.Configuration;
    using System.Text.RegularExpressions;

    public class CodeReplacementElement : ConfigurationElement
    {
        private Regex replacementRegex;

        [ConfigurationProperty("replacementRegex", IsRequired = true)]
        public string ReplacementRegex
        {
            get
            {
                return (string)this["replacementRegex"];
            }

            set
            {
                this["replacementRegex"] = value;
            }
        }

        [ConfigurationProperty("replacementValue", IsRequired = true)]
        public string ReplacementValue
        {
            get
            {
                return (string)this["replacementValue"];
            }

            set
            {
                this["replacementValue"] = value;
            }
        }

        [ConfigurationProperty("skipCommand", IsRequired = false)]
        public string SkipCommand
        {
            get
            {
                return (string)this["skipCommand"];
            }

            set
            {
                this["skipCommand"] = value;
            }
        }

        public string DoReplacementIfApplicable(string command, string text)
        {
            if (this.SkipCommand.Equals(command) || string.IsNullOrEmpty(this.ReplacementRegex))
            {
                return text;
            }

            if (this.replacementRegex == null)
            {
                this.replacementRegex = new Regex(this.ReplacementRegex, RegexOptions.Compiled);
            }

            return this.replacementRegex.Replace(text, this.ReplacementValue);
        }
    }
}