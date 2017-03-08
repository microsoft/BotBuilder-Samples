namespace SourceTool
{
    using System.Configuration;

    public class CodeReplacementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CodeReplacementElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CodeReplacementElement)element).ReplacementRegex + ((CodeReplacementElement)element).ReplacementValue;
        }
    }
}