namespace RollerSkillBot
{
    using System.Xml.Linq;

    public static class SSMLHelper
    {
        public static string Speak(string text)
        {
            var ssml = new XDocument(new XElement(
                "speak",
                new XAttribute("version", "1.0"),
                new XAttribute(XNamespace.Xml + "lang", "en-US"), 
                text));

            return ssml.ToString();
        }

        public static string Emphasis(string text)
        {
            var ssml = new XElement("emphasis", text);
            return ssml.ToString();
        }

        public static string SayAs(string interpretAs, string text)
        {
            var ssml = new XElement(
                "say-as",
                new XAttribute("interpret-as", interpretAs), 
                text);

            return ssml.ToString();
        }
    }
}