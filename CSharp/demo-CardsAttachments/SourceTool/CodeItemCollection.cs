namespace SourceTool
{
    using System.Configuration;

    public class CodeItemCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CodeItemElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CodeItemElement)element).TypeRegex;
        }
    }
}