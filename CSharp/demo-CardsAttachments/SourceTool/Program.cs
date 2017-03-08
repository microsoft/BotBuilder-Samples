namespace SourceTool
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class Program
    {
        private static readonly Regex CodeStartCleanUp = new Regex($"^{Environment.NewLine}\\s*", RegexOptions.Compiled);
        private static readonly Regex CodeLinesCleanUp = new Regex($"{Environment.NewLine}\\s{{12}}", RegexOptions.Compiled);

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Provide all arguments: [sources path] [output path]");
                return;
            }

            var config = (CodeItemsSection)ConfigurationManager.GetSection("codeItemsSection");

            string sourcesPath = args[0];
            string outputPath = args[1];

            var csharpCode = new XElement("code");
            csharpCode.Add(new XAttribute("lang", "C#"));

            var files = Directory.GetFiles(sourcesPath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileText = File.ReadAllText(file);

                foreach (CodeItemElement codeItem in config.CodeItems)
                {
                    var commandMatch = codeItem.GetTypeRegex().Match(fileText);
                    if (commandMatch.Success)
                    {
                        var codeMatch = codeItem.GetCodeRegex().Match(fileText);
                        if (codeMatch.Success)
                        {
                            var element = new XElement("command");
                            element.SetAttributeValue("type", commandMatch.Groups[1].Value);

                            var codeText = CodeStartCleanUp.Replace(CodeLinesCleanUp.Replace(codeMatch.Groups[1].Value, Environment.NewLine), string.Empty);
                            foreach (CodeReplacementElement replacement in codeItem.Replacements)
                            {
                                codeText = replacement.DoReplacementIfApplicable(commandMatch.Groups[1].Value, codeText);
                            }

                            element.ReplaceNodes(new XCData(codeText));

                            csharpCode.Add(element);
                        }
                    }
                }
            }

            csharpCode.Save(Path.Combine(outputPath, "CSharpCode.xml"));

            // compare with node assets and warn not found!
            var nodeJsCode = XElement.Load(Path.Combine(outputPath, "NodeJsCode.xml"));

            foreach (var notFound in csharpCode.Nodes().Except(nodeJsCode.Nodes(), new CodeNodeComparer()))
            {
                var msg = $"Command {(notFound as XElement).Attribute("type")} not found in node assets!";
                Trace.TraceWarning(msg);
            }
        }
    }
}