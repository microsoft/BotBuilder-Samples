namespace SourceTool
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    internal class CodeNodeComparer : IEqualityComparer<XNode>
    {
        public bool Equals(XNode x, XNode y)
        {
            return (x as XElement).Attribute("type").ToString().Equals((y as XElement).Attribute("type").ToString());
        }

        public int GetHashCode(XNode obj)
        {
            return (obj as XElement).Attribute("type").ToString().GetHashCode();
        }
    }
}