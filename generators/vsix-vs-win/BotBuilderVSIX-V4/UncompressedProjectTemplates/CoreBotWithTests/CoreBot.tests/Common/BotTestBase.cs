// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace $ext_safeprojectname$.Tests.Common
{
    /// <summary>
    /// A base class with helper methods and properties to write bot tests.
    /// </summary>
    public abstract class BotTestBase
    {
        // A lazy configuration object that gets instantiated once during execution when is needed
        private static readonly Lazy<IConfiguration> _configurationLazy = new Lazy<IConfiguration>(() =>
        {
            LoadLaunchSettingsIntoEnvVariables("Properties//launchSettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            return config.Build();
        });

        /// <summary>
        /// Initializes a new instance of the <see cref="BotTestBase"/> class.
        /// </summary>
        protected BotTestBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BotTestBase"/> class.
        /// </summary>
        /// <param name="output">
        /// An XUnit <see cref="ITestOutputHelper"/> instance.
        /// See <see href="https://xunit.net/docs/capturing-output.html">Capturing Output</see> in the XUnit documentation for additional details.
        /// </param>
        protected BotTestBase(ITestOutputHelper output)
        {
            Output = output;
        }

        public virtual IConfiguration Configuration => _configurationLazy.Value;

        protected ITestOutputHelper Output { get; }

        /// <summary>
        /// Test runners don't load environment variables defined in launchSettings.json
        /// so this helper code loads it manually if the file is present.
        /// This is useful to be able to have your own key files in your local machine without
        /// having to put them in git.
        /// If you use launch settings, make sure you set the Copy to Output Directory property to Copy Always.
        /// </summary>
        /// <param name="launchSettingsFile">The relative path to the launch settings file (i.e.: "Properties//launchSettings.json")</param>
        private static void LoadLaunchSettingsIntoEnvVariables(string launchSettingsFile)
        {
            if (!File.Exists(launchSettingsFile))
            {
                return;
            }

            using (var file = File.OpenText(launchSettingsFile))
            {
                var reader = new JsonTextReader(file);
                var fileData = JObject.Load(reader);

                var variables = fileData
                    .GetValue("profiles")
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                {
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }
        }
    }
}
