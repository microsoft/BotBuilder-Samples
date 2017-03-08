namespace SourceTool
{
    using System.Configuration;
    using System.Text.RegularExpressions;

    public class CodeItemElement : ConfigurationElement
    {
        private Regex typeRegex;
        private Regex codeRegex;

        [ConfigurationProperty("typeRegex", IsRequired = true)]
        public string TypeRegex
        {
            get
            {
                return (string)this["typeRegex"];
            }

            set
            {
                this["typeRegex"] = value;
            }
        }

        [ConfigurationProperty("codeRegex", IsRequired = true)]
        public string CodeRegex
        {
            get
            {
                return (string)this["codeRegex"];
            }

            set
            {
                this["codeRegex"] = value;
            }
        }

        [ConfigurationProperty("replacements")]
        public CodeReplacementCollection Replacements
        {
            get
            {
                return (CodeReplacementCollection)this["replacements"];
            }

            set
            {
                this["replacements"] = value;
            }
        }

        public Regex GetTypeRegex()
        {
            if (this.typeRegex == null)
            {
                this.typeRegex = new Regex(this.TypeRegex, RegexOptions.Compiled);
            }

            return this.typeRegex;
        }

        public Regex GetCodeRegex()
        {
            if (this.codeRegex == null)
            {
                this.codeRegex = new Regex(this.CodeRegex, RegexOptions.Compiled);
            }

            return this.codeRegex;
        }
    }
}