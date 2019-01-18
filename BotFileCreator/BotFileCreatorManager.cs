using BotFileCreator.BotFileWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BotFileCreator
{
    public class BotFileCreatorManager
    {
        private readonly string BotFileName;

        private readonly string ProjectDirectoryPath;

        private readonly string ProjectName;

        public BotFileCreatorManager(string BotFileName, string ProjectName)
        {
            this.BotFileName = BotFileName;
            this.ProjectName = ProjectName;
            ProjectDirectoryPath = GetProjectDirectoryPath();
        }

        /// <summary>
        /// Creates a '.bot' file in a specific Project
        /// </summary>
        /// <returns>Tuple with bool which specifies if the file was created or not, and a string with the error message (empty string if there isn't any error)</returns>
        public Tuple<bool, string> CreateBotFile()
        {
            Tuple<bool, string> botFileIsValid = BotFileConfigurationIsValid();

            // If the bot file is not valid, will return the corresponding error.
            if (!botFileIsValid.Item1)
            {
                return botFileIsValid;
            }

            // Bot's name, with the '.bot' extension
            string botFileFullName = this.GetBotFileName();

            // bot file's full pathName
            string fullName = Path.Combine(ProjectDirectoryPath, botFileFullName);

            // If the file does not exist, will create it and add to the .csproj file
            if (!File.Exists(fullName))
            {
                WriteBotFileContent(fullName);
                AddFileToProject(this.ProjectName, fullName);
            }
            else
            {
                // Returns an error if the bot file allready exists.
                return new Tuple<bool, string>(false, $"The bot file {botFileFullName} allready exists.");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Returns a tuple with a bool which specifies if the bot file configuration is valid, and a string with the error message (empty string if there isn't any error).
        /// </summary>
        /// <returns>Tuple with bool and string</returns>
        private Tuple<bool, string> BotFileConfigurationIsValid()
        {
            // If the .bot file name is Null or WhiteSpace, returns an error.
            if (string.IsNullOrWhiteSpace(this.BotFileName))
            {
                return new Tuple<bool, string>(false, "Bot file name can't be null.");
            }

            // If the .bot file name contains any whitespace, the method will return an error.
            if (this.BotFileName.Contains(" "))
            {
                return new Tuple<bool, string>(false, "Bot file name can't have whitespaces.");
            }

            // A tuple with True and Empty string will be returned if there are no errors.
            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Add the '.bot' extension to the bot file's name if it doesn't have it.
        /// </summary>
        /// <returns>Bot File name</returns>
        private string GetBotFileName()
        {
            return this.BotFileName.EndsWith(".bot") ? this.BotFileName : string.Concat(this.BotFileName, ".bot");
        }

        /// <summary>
        /// Returns the Working Project Directory
        /// </summary>
        /// <returns>Project Directory</returns>
        private string GetProjectDirectoryPath()
        {
            return this.ProjectName.Substring(0, this.ProjectName.LastIndexOf('\\'));
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
            
            // Reevaluates the project to add any change
            project.ReevaluateIfNecessary();

            // Checks if the project has a file with the same name. If it doesn't, it will be added to the project
            if (project.Items.FirstOrDefault( item => item.EvaluatedInclude == fileName) == null)
            {
                project.AddItem("Compile", fileName);
                project.Save();
            }
        }

        /// <summary>
        /// Writes the content of the bot file.
        /// </summary>
        /// <param name="filePath">Bot file's path</param>
        private void WriteBotFileContent(string filePath)
        {
            // Creates a botFile object for writing its content to the just recently created file
            BotFile botFile = new BotFile();
            botFile.name = this.BotFileName;
            botFile.version = "1.0";
            botFile.description = string.Empty;
            botFile.padlock = string.Empty;
            botFile.services = Enumerable.Empty<BotService>().ToList();

            // Writes the content of the botFile
            BotFileWriterManager.WriteBotFile(botFile, filePath);
        }
    }
}
