namespace SourceTool
{
    using System.Configuration;

    public class CodeItemsSection : ConfigurationSection
    {
        [ConfigurationProperty("codeItems")]
        public CodeItemCollection CodeItems
        {
            get
            {
                return (CodeItemCollection)this["codeItems"];
            }

            set
            {
                this["codeItems"] = value;
            }
        }
    }
}