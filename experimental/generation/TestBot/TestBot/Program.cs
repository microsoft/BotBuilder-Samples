using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Luis.Testing;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Testing;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Extensions.Configuration;

namespace TestBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var root = ".";
            var region = "westus";
            var environment = Environment.UserName;
            for (var i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "--root": root = NextArg(ref i, args); break;
                    case "--region": region = NextArg(ref i, args); break;
                    case "--environment": environment = NextArg(ref i, args); break;
                    default: Usage(); break;
                }
            }

            ComponentRegistration.Add(new DeclarativeComponentRegistration());
            ComponentRegistration.Add(new AdaptiveComponentRegistration());
            ComponentRegistration.Add(new LanguageGenerationComponentRegistration());
            ComponentRegistration.Add(new AdaptiveTestingComponentRegistration());
            ComponentRegistration.Add(new LuisComponentRegistration());
            ComponentRegistration.Add(new QnAMakerComponentRegistration());

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "root", root }, { "region", region }, { "environment", environment } })
                .UseMockLuisSettings(root, "TestBot")
                .AddUserSecrets("RunBot")
                .Build();
            var resourceExplorer = new ResourceExplorer().AddFolder(root, monitorChanges: false);
            resourceExplorer.RegisterType(LuisAdaptiveRecognizer.Kind, typeof(MockLuisRecognizer), new MockLuisLoader(config));

            var dialogs = resourceExplorer.GetResources(".dialog").ToList();
            foreach (var test in resourceExplorer.GetResources(".dialog").Where(r => r.Id.EndsWith(".test.dialog")))
            {
                try
                {
                    Console.WriteLine($"Running test {test.Id}");
                    var script = resourceExplorer.LoadType<TestScript>(test.Id);
                    script.Configuration = config;
                    script.Description ??= test.Id;
                    await script.ExecuteAsync(resourceExplorer, test.Id).ConfigureAwait(false);
                    Console.WriteLine("Passed\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"*** Failed: {e.Message}\n");
                }
            }
        }

        private static void Usage()
        {
            Console.Error.WriteLine("TestBot [--root <rootDir>] [--region <region>] [--environment <settingEnvironment>]");
            Console.Error.WriteLine("Run all *.test.dialog scripts run against dialogs in the root directory with LUIS record and then mock.");
            Console.Error.WriteLine("--root Root directory to use or . by default.");
            Console.Error.WriteLine("--region Region to use for settings defaults to westus.");
            Console.Error.WriteLine("--environment Environment name to use when looking for settings defaults to user alias.");
            Console.Error.WriteLine("To use LUIS you should do 'dotnet user-secrets --id RunBot set luis:endpointKey <yourKey>'");
            System.Environment.Exit(-1);
        }

        private static string NextArg(ref int i, string[] args, bool optional = false, bool allowCmd = false)
        {
            string arg = null;
            if (i < args.Length)
            {
                arg = args[i];
                if (arg.StartsWith("{"))
                {
                    while (!args[i].EndsWith("}") && ++i < args.Length)
                    {
                    }

                    ++i;
                }

                arg = null;
                if (allowCmd)
                {
                    if (i < args.Length)
                    {
                        arg = args[i];
                    }
                }
                else
                {
                    if (i < args.Length && !args[i].StartsWith('-'))
                    {
                        arg = args[i];
                    }
                    else if (!optional)
                    {
                        Usage();
                    }
                    else
                    {
                        --i;
                    }
                }
            }

            return arg?.Trim();
        }
    }
}
