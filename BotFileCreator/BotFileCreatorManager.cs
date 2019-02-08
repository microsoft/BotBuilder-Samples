using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotFileCreator
{
    public class BotFileCreatorManager
    {
        private BotFileNameManager botFileNameManager;
        private MSBotCommandManager commandManager;
        private BotConfigurationViewModel botConfigurationViewModel;

        public BotFileCreatorManager(BotFileNameManager botFileNameManager, MSBotCommandManager commandManager, BotConfigurationViewModel botConfigurationViewModel)
        {
            this.botFileNameManager = botFileNameManager;
            this.commandManager = commandManager;
            this.botConfigurationViewModel = botConfigurationViewModel;
        }

        /// <summary>
        /// Creates a '.bot' file in a specific Project
        /// </summary>
        /// <returns>Tuple with bool which specifies if the file was created or not, and a string with the error message (empty string if there isn't any error)</returns>
        public Tuple<bool, string> CreateBotFile()
        {
            // bot file's full pathName
            string fullName = Path.Combine(botFileNameManager.ProjectDirectoryPath, botFileNameManager.BotFileNameWithExtension);

            // If the file does not exist, will create it and add to the .csproj file
            if (!File.Exists(fullName))
            {
                var output = string.Empty;
                CreateBotFileFromCMD(this.commandManager.GetMSBotCommand(), out output);

                if (!string.IsNullOrWhiteSpace(output))
                {
                    var outputJsonObject = (JObject)JsonConvert.DeserializeObject(output);
                    botConfigurationViewModel.SecretKey = (string)outputJsonObject["secret"];
                }

                AddFileToProject(botFileNameManager.ProjectName, fullName);
            }
            else
            {
                // Returns an error if the bot file already exists.
                return new Tuple<bool, string>(false, $"The bot file {botFileNameManager.BotFileNameWithExtension} already exists.");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Adds a specified file to another specified project
        /// </summary>
        /// <param name="projectName">The full path to .csproj file</param>
        /// <param name="fileName">The file name to add to csproj</param>
        private void AddFileToProject(string projectName, string fileName)
        {
            // Load a specific project. Also, avoids several problems for re-loading the same project more than once
            var project = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == projectName);

            if (project != null)
            {
                // Reevaluates the project to add any change
                project.ReevaluateIfNecessary();

                // Checks if the project has a file with the same name. If it doesn't, it will be added to the project
                if (project.Items.FirstOrDefault(item => item.EvaluatedInclude == fileName) == null)
                {
                    project.AddItem("Compile", fileName);
                    project.Save();
                }
            }
        }

        /// <summary>
        /// Creates a .bot file using the command line
        /// </summary>
        /// <param name="command">Command for creating bot files</param>
        /// <param name="commandOutput">out result of command.</param>
        private void CreateBotFileFromCMD(string command, out string commandOutput)
        {
            CLIHelper.CLIHelper.RunCommand(command, out commandOutput);
        }
    }
}
